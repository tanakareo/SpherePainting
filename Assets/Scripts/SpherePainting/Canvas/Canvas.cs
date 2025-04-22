using System;
using UnityEngine;
using R3;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

namespace SpherePainting
{
    public class Canvas : MonoBehaviour
    {
        // キャンバスの形状
        public enum ShapeType { ELLIPSE, RECTANGLE }

        // 最小値と最大値
        public static readonly int MIN_LAYERS = 1;
        public static readonly int MAX_LAYERS = 1000;
        public static readonly float MIN_SIZE = 0.1f;
        public static readonly float MAX_SIZE = 20.0f;
        public static readonly float MIN_DEPTH = 0.01f;
        public static readonly float MAX_DEPTH = 10.0f;
        //　キャンバスのモデルの厚さ
        private static readonly float BASE_DEPTH = 0.25f;
        //　モデルのサイズ
        private static readonly float MODEL_SIZE = 2.5f;

        [SerializeField] private FinalRendering m_FinalRendering;
        [SerializeField] private RenderResult m_RenderResult;
        [SerializeField, SerializedDictionary("ShapeType", "CanvasMesh")] private SerializedDictionary<ShapeType, CanvasMesh> m_CanvasMeshes;
        [SerializeField] private GameObject m_BaseGameObject;
        [SerializeField] private GameObject m_LayerPrefab;
        private GameObject[] m_LayerGameObject = new GameObject[MAX_LAYERS];
        private MeshFilter[] m_LayerMeshFilters = new MeshFilter[MAX_LAYERS];
        private MeshRenderer[] m_LayerMeshRenderers = new MeshRenderer[MAX_LAYERS];

        // UIから操作するプロパティ
        private readonly ReactiveProperty<ShapeType> m_CurrentShapeType = new (ShapeType.ELLIPSE);
        private readonly ReactiveProperty<float> m_Depth = new (0.01f);
        private readonly ReactiveProperty<Vector2> m_Size = new (new Vector2(2.5f, 2.5f));
        private readonly ReactiveProperty<bool> m_IsLayerFilterActive = new (false);
        private readonly ReactiveProperty<int> m_ActiveFilteredLayer = new (0);
        private readonly ReactiveProperty<bool> m_IsLayerShuffleActive = new (false);
        private readonly ReactiveProperty<uint> m_LayerShuffleSeed = new (1);
        private readonly ReactiveProperty<Vector2> m_NormalizedTextureOffset = new (Vector2.one * 0.5f);
        private readonly ReactiveProperty<float> m_TextureScaler = new (1.0f);

        public ReadOnlyReactiveProperty<ShapeType> CurrentShapeType => m_CurrentShapeType;
        public ReadOnlyReactiveProperty<float> Depth => m_Depth;
        public ReadOnlyReactiveProperty<Vector2> Size => m_Size;
        public ReadOnlyReactiveProperty<bool> IsLayerFilterActive => m_IsLayerFilterActive;
        public ReadOnlyReactiveProperty<int> ActiveFilteredLayer => m_ActiveFilteredLayer;
        public ReadOnlyReactiveProperty<bool> IsLayerShuffleActive => m_IsLayerShuffleActive;
        public ReadOnlyReactiveProperty<uint> LayerShuffleSeed => m_LayerShuffleSeed;
        public ReadOnlyReactiveProperty<Vector2> NormalizedTextureOffset => m_NormalizedTextureOffset;
        public ReadOnlyReactiveProperty<float> TextureScaler => m_TextureScaler;

        private readonly ReactiveProperty<int> m_LayerCount = new (1);
        public ReadOnlyReactiveProperty<int> LayerCount => m_LayerCount;

        // 正規化していない実際のテクスチャスケール、オフセット
        private readonly ReactiveProperty<Vector2> m_CurrentTextureScale = new (Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> CurrentTextureScale => m_CurrentTextureScale;
        private readonly ReactiveProperty<Vector2> m_CurrentTextureOffset = new (Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> CurrentTextureOffset => m_CurrentTextureOffset;
        private bool m_IsTextureMappingChanged = false;
        public int[] OriginalLayerIndices { get; private set; } = new int[MAX_LAYERS];
        public int[] CurrentLayerIndices { get; private set; }  = new int[MAX_LAYERS];
        private bool m_IsLayerShuffled = false;
        
        private void Awake()
        {
            InstantiateLayers();
            InitLayerIndices();
            AdjustLayerSpacing();
            ChangeCanvasShapeType(m_CurrentShapeType.Value);
            if(m_IsLayerFilterActive.Value)
            {
                FilterLayer(m_ActiveFilteredLayer.Value);
            }
            else
            {
                ShowActiveLayers();
            }

            m_FinalRendering.OnStartRendering += (info) =>
            {
                SetLayerCount(info.LayerCount);
                SetTextures(m_RenderResult.RenderTextures);
            };
        }

        // レイヤーのインデックス配列を初期化
        private void InitLayerIndices()
        {
            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                OriginalLayerIndices[i] = i;
                CurrentLayerIndices[i] = i;
            }
        }

        private void Update()
        {
            // テクスチャマッピングを更新
            if(m_IsTextureMappingChanged)
            {
                UpdateTextureMapping();
            }

            // レイヤーの順番を更新
            if(m_IsLayerShuffled)
            {
                UpdateLayerOrder();
            }
        }

        // キャンバスの奥行きを設定
        public void SetDepth(float depth)
        {
            if(depth < MIN_DEPTH || depth > MAX_DEPTH) return;

            m_Depth.Value = depth;
            AdjustLayerSpacing();
        }

        // キャンバスのレイヤー数を設定
        private void SetLayerCount(int layerCount)
        {
            if(layerCount < MIN_LAYERS || layerCount > MAX_LAYERS) return;

            m_LayerCount.Value = layerCount;

            if(m_IsLayerShuffleActive.Value) ShuffleLayer(m_LayerShuffleSeed.Value);

            AdjustLayerSpacing();

            if(m_IsLayerFilterActive.Value)
            {
                FilterLayer(Mathf.Min(m_ActiveFilteredLayer.Value, layerCount - 1));
                return;
            }

            ShowActiveLayers();
        }

        // 制限されたキャンバスの大きさを取得
        private Vector2 GetClampedSize(Vector2 size)
        {
            Vector2 clampedSize = new Vector2(Mathf.Clamp(size.x, MIN_SIZE, MAX_SIZE), Mathf.Clamp(size.y, MIN_SIZE, MAX_SIZE));
            return clampedSize;
        }


        // キャンバスの大きさを設定
        public void SetSize(Vector2 size)
        {
            m_Size.Value = GetClampedSize(size);
            Vector2 newSize = m_Size.Value / MODEL_SIZE;
            m_BaseGameObject.transform.localScale = new Vector3(newSize.x, newSize.y, m_BaseGameObject.transform.localScale.z);

            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                Transform layerTransform = GetLayer(i).transform;
                layerTransform.localScale = new Vector3(newSize.x, newSize.y, layerTransform.localScale.z);
            }

            m_IsTextureMappingChanged = true;
        }

        //　レイヤーのプレファブをインスタンス化
        private void InstantiateLayers()
        {
            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                m_LayerGameObject[i] = Instantiate(m_LayerPrefab, transform);
                m_LayerMeshFilters[i] = m_LayerGameObject[i].GetComponent<MeshFilter>();
                m_LayerMeshRenderers[i] = m_LayerGameObject[i].GetComponent<MeshRenderer>();
            }
        }

        // レイヤーの間隔を調整
        private void AdjustLayerSpacing()
        {
            Vector3 origin = transform.position + Vector3.forward * (m_Depth.Value + BASE_DEPTH);
            float layerSpacing = m_Depth.Value / m_LayerCount.Value;

            for(int i = 0; i < m_LayerCount.Value; ++i)
            {
                Transform layerTransform = GetLayer(i).transform;
                layerTransform.position = origin + Vector3.back * layerSpacing * (i + 1);
                Vector3 localScale = layerTransform.localScale;
                localScale.z = layerSpacing;
                layerTransform.localScale = localScale;
            }
        }

        // インデックスからレイヤーを取得
        private GameObject GetLayer(int originalIndex)
        {
            return m_LayerGameObject[CurrentLayerIndices[originalIndex]];
        }

        //　指定した番号のレイヤーのみを表示
        public void FilterLayer(int layerIndex)
        {
            if(layerIndex < 0 || layerIndex >= m_LayerCount.Value) return;

            m_IsLayerFilterActive.Value = true;
            m_ActiveFilteredLayer.Value = layerIndex;

            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                if(i == layerIndex) GetLayer(i).SetActive(true);
                else GetLayer(i).SetActive(false);
            }
        }

        // 現在のレイヤー数をもとにレイヤーの表示を設定
        public void ShowActiveLayers()
        {
            m_IsLayerFilterActive.Value = false;

            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                if(i < m_LayerCount.Value) GetLayer(i).SetActive(true);
                else GetLayer(i).SetActive(false);
            }
        }

        // レイヤーにテクスチャを割り当て
        public void SetTextures(IReadOnlyList<RenderTexture> renderTextures)
        {
            for(int i = 0; i < renderTextures.Count; ++i)
            {
                SetTexture(renderTextures[i], i);
            }
        }

        // 指定のレイヤーにテクスチャを割り当て
        private void SetTexture(RenderTexture renderTexture, int layerIndex)
        {
            Material layerMat = m_LayerMeshRenderers[layerIndex].material;
            layerMat.mainTexture = renderTexture;
        }

        public void SetTextureOffset(Vector2 offset)
        {
            m_NormalizedTextureOffset.Value = offset;
            m_IsTextureMappingChanged = true;
        }

        public void SetTextureScale(float scale)
        {
            m_TextureScaler.Value = scale;
            m_IsTextureMappingChanged = true;
        }

        // 正規化されたテクスチャオフセットをもとにテクスチャのオフセットを更新
        private void UpdateTextureOffset()
        {
            string textureName = "_MainTex";

            // 正規化されたテクスチャオフセットをもとに実際のテクスチャのオフセットを計算
            Vector2 textureOffset = new Vector2(Mathf.Lerp(-0.5f + m_CurrentTextureScale.Value.x * 0.5f, 0.5f - m_CurrentTextureScale.Value.x * 0.5f, m_NormalizedTextureOffset.Value.x),
                                                Mathf.Lerp(-0.5f + m_CurrentTextureScale.Value.y * 0.5f, 0.5f - m_CurrentTextureScale.Value.y * 0.5f, m_NormalizedTextureOffset.Value.y));
            m_CurrentTextureOffset.Value = textureOffset + (Vector2.one - m_CurrentTextureScale.Value) * 0.5f;
            m_CurrentTextureOffset.Value = new Vector2(Mathf.Clamp(m_CurrentTextureOffset.Value.x, 0, 1 - m_CurrentTextureScale.Value.x)
                                              ,Mathf.Clamp(m_CurrentTextureOffset.Value.y, 0, 1 - m_CurrentTextureScale.Value.y));

            // 計算結果をもとにマテリアルのテクスチャオフセットを設定
            for (int i = 0; i < MAX_LAYERS; ++i)
            {
                Material layerMat = m_LayerMeshRenderers[i].material;
                layerMat.SetTextureOffset(textureName, m_CurrentTextureOffset.Value);
            }
        }

        // サイズの縦横比、拡大率からテクスチャのスケールを更新
        private void UpdateTextureScale()
        {
            // サイズの縦横比、拡大率からテクスチャのスケールを計算
            if(m_Size.Value.x > m_Size.Value.y)
            {
                m_CurrentTextureScale.Value = new Vector2(1.0f, m_Size.Value.y / m_Size.Value.x) * m_TextureScaler.Value;
            }
            else
            {
                m_CurrentTextureScale.Value = new Vector2(m_Size.Value.x / m_Size.Value.y, 1.0f) * m_TextureScaler.Value;
            }
            string textureName = "_MainTex";
            // 計算結果をもとにマテリアルのテクスチャスケールを設定
            for(int i = 0; i < MAX_LAYERS; ++i)
            {
                Material layerMat = m_LayerMeshRenderers[i].material;
                layerMat.SetTextureScale(textureName, m_CurrentTextureScale.Value);
            }
        }

        //　テクスチャマッピングを更新
        private void UpdateTextureMapping()
        {
            UpdateTextureScale();
            UpdateTextureOffset();
            m_IsTextureMappingChanged = false;
        }

        //　レイヤーの順番を更新
        private void UpdateLayerOrder()
        {
            AdjustLayerSpacing();
            m_IsLayerShuffled = false;
        }

        public void ChangeCanvasShapeType(ShapeType shapeType)
        {
            m_CurrentShapeType.Value = shapeType;
            CanvasMesh canvasMesh = m_CanvasMeshes[shapeType];
            m_BaseGameObject.GetComponent<MeshFilter>().mesh = canvasMesh.BaseMesh;

            foreach(var meshFilter in m_LayerMeshFilters)
            {
                meshFilter.mesh = canvasMesh.LayerMesh;
            }

            SetSize(m_Size.Value);
        }

        public void ShuffleLayer(uint seed)
        {
            m_IsLayerShuffleActive.Value = true;
            m_LayerShuffleSeed.Value = seed;
            Array.Copy(OriginalLayerIndices, CurrentLayerIndices, MAX_LAYERS);
            ArrayShuffler.ShuffleRange(m_LayerShuffleSeed.Value, CurrentLayerIndices, 0, m_LayerCount.Value - 1);

            AdjustLayerSpacing();

            // フィルタリングされている場合は、再度フィルタリングをする。
            if(m_IsLayerFilterActive.Value)
            {
                FilterLayer(m_ActiveFilteredLayer.Value);
            }
        }

        // レイヤーの順番を元に戻す
        public void UnshuffleLayer()
        {
            m_IsLayerShuffleActive.Value = false;
            Array.Copy(OriginalLayerIndices, CurrentLayerIndices, MAX_LAYERS);
            AdjustLayerSpacing();
        }
    }
}