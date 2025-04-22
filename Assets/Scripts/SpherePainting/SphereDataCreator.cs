using Random = Unity.Mathematics.Random;
using R3;
using UnityEngine;
using System;
using System.Collections.Generic;
using WeightedValues;

namespace SpherePainting
{
    public class SphereDataCreator : MonoBehaviour
    {
        // シード値
        public ReadOnlyReactiveProperty<uint> Seed => m_Seed;
        private readonly ReactiveProperty<uint> m_Seed = new (1);
        public void SetSeed(uint seed)
        {
            seed = Math.Max(1, seed);
            m_Seed.Value = seed;
        }

        // 球の演算と球のファクトリのディクショナリ
        public Dictionary<SphereOperation, ISphereFactory> SphereFactories => m_SphereFactories;
        private Dictionary<SphereOperation, ISphereFactory> m_SphereFactories = new ()
        {
            {SphereOperation.NORMAL, new SphereFactory(new (0.0f, 100.0f, 0.0f, 5.0f))},
            {SphereOperation.CAP, new OperatedSphereFactory(new (0.0f, 100.0f, 0.0f, 5.0f), new (0.0f, 2.0f, 0.2f, 1.0f), new (1, 50, 1, 2), SphereOperation.CAP)},
            {SphereOperation.DIFFERENCE, new OperatedSphereFactory(new (0.0f, 100.0f, 0.0f, 5.0f), new (0.0f, 2.0f, 0.0f, 0.8f), new (1, 50, 1, 5), SphereOperation.DIFFERENCE)},
        };

        // 球の演算の割合
        public ReactiveWeightedValues<SphereOperation> WeightedSphereOperations { get; private set; } = new (
            new WeightedItem<SphereOperation>(SphereOperation.NORMAL, 1.0f),
            new WeightedItem<SphereOperation>(SphereOperation.CAP, 1.0f),
            new WeightedItem<SphereOperation>(SphereOperation.DIFFERENCE, 1.0f)
        );

        // 球の生成領域
        public SphereGenerationBounds GenerationBounds => m_GenerationBounds;
        private readonly SphereGenerationBounds m_GenerationBounds = new ();

        // 球の数
        public ReadOnlyReactiveProperty<uint> SphereCount => m_SphereCount;
        private readonly ReactiveProperty<uint> m_SphereCount = new (5);
        public void SetSphereCount(uint sphereCount)
        {
            sphereCount = Math.Max(1, sphereCount);
            m_SphereCount.Value = sphereCount;
        }

        public event Action OnSpheresParameterChanged; // 球のパラメータ変更時に呼ぶ
        public event Action OnOperationTargetSpheresParameterChanged; // 演算対象の球のパラメータ変更時に呼ぶ

        [SerializeField] private SphereDataListContainer m_SphereDataListContainer;
        [SerializeField] private SpheresGraphicsBufferCreator m_SpheresGraphicsBufferCreator;

        private bool m_IsSpheresParameterChanged = true;
        private bool m_IsOperationTargetSpheresParameterChanged = true;

        private void Awake()
        {
            Init();
        }

        // 初期化
        private void Init()
        {
            GenerationBounds.OnValueChanged += v => {
                m_IsSpheresParameterChanged = true;
            };
            WeightedSphereOperations.Subscribe(v =>
            {
                m_IsSpheresParameterChanged = true;
                m_IsOperationTargetSpheresParameterChanged = true;
            }).AddTo(this);

            foreach(var (operation, sphereFactory) in m_SphereFactories)
            {
                sphereFactory.RadiusRange.Subscribe(v =>
                {
                    m_IsSpheresParameterChanged = true;
                    m_IsOperationTargetSpheresParameterChanged = true;
                }).AddTo(this);

                if(sphereFactory is OperatedSphereFactory operatedSphereFactory)
                {
                    operatedSphereFactory.OperationTargetCountRange.Subscribe(v =>
                    {
                        m_IsSpheresParameterChanged = true;
                        m_IsOperationTargetSpheresParameterChanged = true;
                    }).AddTo(this);
                    operatedSphereFactory.OperationTargetRadiusRange.Subscribe(v =>
                    {
                        m_IsOperationTargetSpheresParameterChanged = true;
                    }).AddTo(this);
                }
            }
            Seed.Subscribe(v =>
            {
                m_IsSpheresParameterChanged = true;
                m_IsOperationTargetSpheresParameterChanged = true;
            }).AddTo(this);
            SphereCount.Subscribe(v =>
            {
                m_IsSpheresParameterChanged = true;
                m_IsOperationTargetSpheresParameterChanged = true;
            }).AddTo(this);
            
            // 球を生成
            Generate();
        }

        // パラメータが変更されていたら球を再生成
        private void Update()
        {
            if(m_IsSpheresParameterChanged == false && m_IsOperationTargetSpheresParameterChanged == false) return;
            
            if(m_IsSpheresParameterChanged)
            {
                m_SpheresGraphicsBufferCreator.RequireUpdateSpheresBuffer();
                OnSpheresParameterChanged?.Invoke();
            }
            
            if(m_IsOperationTargetSpheresParameterChanged)
            {
                m_SpheresGraphicsBufferCreator.RequireUpdateOperationTargetSpheresBuffer();
                OnOperationTargetSpheresParameterChanged?.Invoke();
            }

            Generate();
        }

        // 球を生成
        private void Generate()
        {
            m_SphereDataListContainer.ClearAll();
            Random random = new (Seed.CurrentValue);

            // SphereDataListContainerに球のデータをセットする。
            for(int i = 0; i < SphereCount.CurrentValue; ++i)
            {
                Vector3 spherePosition = GenerationBounds.GetRandomPosition(ref random);
                SphereOperation randomOperation = WeightedSphereOperations.GetRandomValue(ref random);
                ISphereFactory sphereFactory = m_SphereFactories[randomOperation];
                SphereData sphereData = m_SphereFactories[randomOperation].GetRandomSphereData(spherePosition, ref random);
                if(sphereFactory is OperatedSphereFactory operatedSphereFactory)
                {
                    sphereData.OperationTargetStartIndex = (uint)m_SphereDataListContainer.OperationTargetSphereDataCount;
                    for(int j = 0; j < sphereData.OperationTargetCount; ++j)
                    {
                        OperationTargetSphereData data = operatedSphereFactory.GetRandomOperationTargetSphereData(sphereData, ref random);
                        data.MaterialIndex = SphereCount.CurrentValue + 2 * (sphereData.OperationTargetStartIndex + (uint)j);
                        data.OperationAreaMaterialIndex = data.MaterialIndex + 1;
                        m_SphereDataListContainer.AddOperationTargetSphereData(data);
                    }
                }
                m_SphereDataListContainer.AddSphereData(sphereData);
            }
            
            // フラグをリセット
            m_IsSpheresParameterChanged = false;
            m_IsOperationTargetSpheresParameterChanged = false;
        }
    }
}
