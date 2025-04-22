using Random = Unity.Mathematics.Random;
using R3;
using UnityEngine;
using System;
using Unity.Mathematics;
using Range;
using WeightedValues;

namespace SpherePainting
{
    public class SphereMaterialDataCreator : MonoBehaviour
    {
        // シード値
        public ReadOnlyReactiveProperty<uint> Seed => m_Seed;
        private readonly ReactiveProperty<uint> m_Seed = new (1);
        public void SetSeed(uint seed)
        {
            seed = Math.Max(1, seed);
            m_Seed.Value = seed;
        }
        // 演算箇所のマテリアルのランダム具合（球のマテリアルと完全にランダムなマテリアルをどのくらいブレンドするか）の範囲
        public ReactiveRange<float> OperationAreaMaterialRandomnessRange { get; private set; } = new (0.0f, 1.0f, 1.0f, 1.0f);
        // 演算対象の球のマテリアルのランダム具合の範囲
        public ReactiveRange<float> OperationTargetMaterialRandomnessRange { get; private set; } = new (0.0f, 1.0f, 1.0f, 1.0f);
        // 球のマテリアルの範囲
        public HSVColorRange MaterialColorRange { get; private set; } = new HSVColorRange();
        // 演算対象の球のマテリアルのランダムの範囲
        public ReactiveRange<float> OperationTargetMaterialOpacityRange { get; private set; }= new (0.0f, 1.0f, 0.0f, 1.0f);
        public ReadOnlyReactiveProperty<float> TransparentOperationTargetRatio => m_TransparentOperationTargetRatio;
        private readonly ReactiveProperty<float> m_TransparentOperationTargetRatio = new (0.5f);
        
        //　透明な演算対象の球の割合
        public void SetTransparentOperationTargetRatio(float ratio)
        {
            m_TransparentOperationTargetRatio.Value = ratio;
        }

        // マテリアルのブレンダモードの割合
        public ReactiveWeightedValues<BlendMode> WeightedBlendModes { get; private set; }
        = new (new WeightedItem<BlendMode>[]{
                new (BlendMode.NORMAL, 0.7f),
                new (BlendMode.ADD, 0.15f),
                new (BlendMode.MULTIPLY, 0.15f),
                new (BlendMode.OVERLAY, 0.15f),
                new (BlendMode.ADD_GLOW, 0.15f),
                new (BlendMode.DIFFERENCE, 0.15f),
                new (BlendMode.EXCLUSION, 0.15f),
                new (BlendMode.HUE, 0.15f),
                new (BlendMode.SATURATION, 0.15f),
                new (BlendMode.COLOR, 0.15f),
                new (BlendMode.LUMINOSITY, 0.15f),
            });

        [SerializeField] private SphereDataCreator m_SphereDataCreator;
        [SerializeField] private SphereDataListContainer m_SphereDataListContainer;
        [SerializeField] private SphereMaterialDataListContainer m_DataListContainer;
        [SerializeField] private SphereMaterialGraphicsBufferCreator m_GraphicsBufferCreator;

        // パラメータが変更されたかどうか
        private bool m_IsParameterChanged = true;

        private void Awake()
        {
            Init();
        }

        // 初期化
        private void Init()
        {
            Seed.Subscribe(v => 
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            WeightedBlendModes.Subscribe(v =>
            {
                m_IsParameterChanged = true;

            }).AddTo(this);

            OperationAreaMaterialRandomnessRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            OperationTargetMaterialRandomnessRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            MaterialColorRange.HueRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);
            MaterialColorRange.SaturationRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);
            MaterialColorRange.ValueRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);
            MaterialColorRange.OpacityRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            OperationTargetMaterialOpacityRange.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            TransparentOperationTargetRatio.Subscribe(v =>
            {
                m_IsParameterChanged = true;
            }).AddTo(this);

            m_SphereDataCreator.OnSpheresParameterChanged += () =>
            {
                m_IsParameterChanged = true;
            };

            m_SphereDataCreator.OnOperationTargetSpheresParameterChanged += () =>
            {
                m_IsParameterChanged = true;
            };
            Observable.EveryUpdate()
                .Where(_ => m_IsParameterChanged)
                .Subscribe(_ =>
                {
                    Create();
                    m_GraphicsBufferCreator.UpdateBuffer();
                }).AddTo(this);
        }

        // 球のマテリアルを作成
        private void Create()
        {
            // データリストをクリア
            m_DataListContainer.Clear();
            Random random = new (Seed.CurrentValue);

            // 球のマテリアルを作成して、データリストに追加
            for(int i = 0; i < m_SphereDataListContainer.SphereDataCount; ++i)
            {
                SphereMaterialData data = SphereMaterialData.GenerateRandom(MaterialColorRange, WeightedBlendModes, ref random);
                m_DataListContainer.AddData(data);
            }
            
            // 演算対象の球のマテリアルを作成して、データリストに追加
            for(int i = 0; i < m_SphereDataListContainer.SphereDataCount; ++i)
            {
                SphereData sphereData = m_SphereDataListContainer.GetSphereData(i);

                // ノーマルの球はスキップ
                if(sphereData.Operation == SphereOperation.NORMAL) continue;

                for(int j = 0; j < sphereData.OperationTargetCount; ++j)
                {
                    SphereMaterialData baseData = m_DataListContainer.GetData(i);
                    float randomness;
                    randomness = OperationTargetMaterialRandomnessRange.GetRandomValue(ref random);
                    SphereMaterialData operationTargetMat = SphereMaterialData.GenerateVariant(baseData, MaterialColorRange, WeightedBlendModes, randomness, ref random);
                    operationTargetMat.BlendMode = baseData.BlendMode;
                    float operationTargetOpacity = OperationTargetMaterialOpacityRange.GetRandomValue(ref random);
                    if(random.NextFloat() <= m_TransparentOperationTargetRatio.Value) operationTargetOpacity = 0.0f;
                    operationTargetMat.Color = new float4(operationTargetMat.Color.xyz, operationTargetOpacity);
                    m_DataListContainer.AddData(operationTargetMat);

                    randomness = OperationAreaMaterialRandomnessRange.GetRandomValue(ref random);
                    SphereMaterialData operationAreaMat = SphereMaterialData.GenerateVariant(baseData, MaterialColorRange, WeightedBlendModes, randomness, ref random);
                    operationAreaMat.BlendMode = baseData.BlendMode;

                    m_DataListContainer.AddData(operationAreaMat);
                }
            }

            // フラグをリセット
            m_IsParameterChanged = false;
        }
    }
}
