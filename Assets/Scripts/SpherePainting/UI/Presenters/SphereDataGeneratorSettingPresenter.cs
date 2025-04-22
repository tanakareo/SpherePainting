using System;
using System.Collections.Generic;
using R3;
using Range;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [RequireComponent(typeof(UIDocument))]
    public class SphereDataCreatorSettingPresenter : MonoBehaviour
    {
        [SerializeField] private SphereDataCreator m_SphereDataCreator;
        private readonly Dictionary<SphereOperation, ISphereSliders> m_SphereSlidersMap = new ();

        private interface ISphereSliders
        {
            public MinMaxSlider RadiusRangeSlider { get; }
            public Slider WeightSlider { get; }
        }

        private class NormalSphereSliders : ISphereSliders
        {
            private readonly MinMaxSlider m_RadiusRangeSlider;
            public MinMaxSlider RadiusRangeSlider => m_RadiusRangeSlider;

            private readonly Slider m_WeightSlider;
            public Slider WeightSlider => m_WeightSlider;

            public NormalSphereSliders(MinMaxSlider radiusRangeSlider, Slider weightSlider)
            {
                m_RadiusRangeSlider = radiusRangeSlider;
                m_WeightSlider = weightSlider;
            }
        }

        private class OperatedSphereSliders : ISphereSliders
        {
            private readonly MinMaxSlider m_RadiusRangeSlider;
            private readonly MinMaxSlider m_TargetRadiusRangeSlider;
            private readonly MinMaxSlider m_TargetCountRangeSlider;
            public MinMaxSlider RadiusRangeSlider => m_RadiusRangeSlider;
            public MinMaxSlider TargetRadiusRangeSlider => m_TargetRadiusRangeSlider;
            public MinMaxSlider TargetCountRangeSlider => m_TargetCountRangeSlider;

            private readonly Slider m_WeightSlider;
            public Slider WeightSlider => m_WeightSlider;

            public OperatedSphereSliders(MinMaxSlider radiusRangeSlider, MinMaxSlider targetRadiusRangeSlider, MinMaxSlider targetCountRangeSlider, Slider weightSlider)
            {
                m_RadiusRangeSlider = radiusRangeSlider;
                m_TargetRadiusRangeSlider = targetRadiusRangeSlider;
                m_TargetCountRangeSlider = targetCountRangeSlider;
                m_WeightSlider = weightSlider;
            }
        }

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // シード値
            var seedSlider = root.Q<SliderInt>("generator-seed-slider-int");
            m_SphereDataCreator.Seed.Subscribe(v =>
            {
                seedSlider.SetValueWithoutNotify((int)v);
            }).AddTo(this);
            seedSlider.RegisterValueChangedCallback(evt => 
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereDataCreator.SetSeed((uint)evt.newValue);
            });

            // 球の数
            var sphereCountSliderInt = root.Q<SliderInt>("sphere-count-slider-int");
            m_SphereDataCreator.SphereCount.Subscribe(v =>
            {
                sphereCountSliderInt.SetValueWithoutNotify((int)v);
            }).AddTo(this);
            sphereCountSliderInt.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereDataCreator.SetSphereCount((uint)evt.newValue);
            });

            // 生成領域
            var boundsSizeVector3Field = root.Q<Vector3Field>("generation-bounds-size-vector3-field");
            var boundsPositionSlider = root.Q<Vector3Field>("generation-bounds-position-vector3-field");
            boundsSizeVector3Field.value = m_SphereDataCreator.GenerationBounds.Size;
            boundsPositionSlider.value = m_SphereDataCreator.GenerationBounds.Position;
            m_SphereDataCreator.GenerationBounds.OnValueChanged += v => 
            {
                boundsSizeVector3Field.SetValueWithoutNotify(v.Size);
                boundsPositionSlider.SetValueWithoutNotify(v.Position);
            };

            boundsSizeVector3Field.OnEndEdit(() =>
            {
                m_SphereDataCreator.GenerationBounds.SetSize(boundsSizeVector3Field.value);
            });
            boundsPositionSlider.OnEndEdit(() =>
            {
                m_SphereDataCreator.GenerationBounds.SetPosition(boundsPositionSlider.value);
            });

            // 球の設定
            var setRadiusIndividualToggle = root.Q<Toggle>("set-radius-individual-toggle");
            var radiusRangeSlider = root.Q<MinMaxSlider>("radius-range-min-max-slider");
            
            var normalSphereRadiusRangeSlider = root.Q<MinMaxSlider>("normal-sphere-radius-range-min-max-slider");
            var capSphereRadiusRangeSlider = root.Q<MinMaxSlider>("cap-sphere-radius-range-min-max-slider");
            var capSphereOperationTargetRadiusRangeSlider = root.Q<MinMaxSlider>("cap-sphere-operation-target-radius-range-min-max-slider");
            var capSphereOperationTargetCountRangeSlider = root.Q<MinMaxSlider>("cap-sphere-operation-target-count-range-min-max-slider");
            var differenceSphereRadiusRangeSlider = root.Q<MinMaxSlider>("difference-sphere-radius-range-min-max-slider");
            var differenceSphereOperationTargetRadiusRangeSlider = root.Q<MinMaxSlider>("difference-sphere-operation-target-radius-range-min-max-slider");
            var differenceSphereOperationTargetCountRangeSlider = root.Q<MinMaxSlider>("difference-sphere-operation-target-count-range-min-max-slider");
            
            var normalSphereWeightSlider = root.Q<Slider>("normal-sphere-weight-slider");
            var capSphereWeightSlider = root.Q<Slider>("cap-sphere-weight-slider");
            var differenceSphereWeightSlider = root.Q<Slider>("difference-sphere-weight-slider");

            m_SphereSlidersMap.Add(SphereOperation.NORMAL, new NormalSphereSliders(normalSphereRadiusRangeSlider, normalSphereWeightSlider));
            m_SphereSlidersMap.Add(SphereOperation.CAP, new OperatedSphereSliders(capSphereRadiusRangeSlider, capSphereOperationTargetRadiusRangeSlider, capSphereOperationTargetCountRangeSlider, capSphereWeightSlider));
            m_SphereSlidersMap.Add(SphereOperation.DIFFERENCE, new OperatedSphereSliders(differenceSphereRadiusRangeSlider, differenceSphereOperationTargetRadiusRangeSlider, differenceSphereOperationTargetCountRangeSlider, differenceSphereWeightSlider));

            var normalSphereSettingFoldout = root.Q<Foldout>("normal-sphere-setting-foldout");

            setRadiusIndividualToggle.value = false;
            radiusRangeSlider.style.display = setRadiusIndividualToggle.value ? DisplayStyle.None : DisplayStyle.Flex;
            normalSphereSettingFoldout.style.display = setRadiusIndividualToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
            setRadiusIndividualToggle.RegisterValueChangedCallback(evt =>
            {
                normalSphereSettingFoldout.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                radiusRangeSlider.style.display = evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
                if(evt.newValue) return;
                if(m_SphereDataCreator.SphereFactories.TryGetValue(SphereOperation.NORMAL, out var normalSphereFactory) == false) return;
                radiusRangeSlider.SetValueWithoutNotify(normalSphereFactory.RadiusRange.ToVector2());
            });

            foreach(var (operation, sphereSliders) in m_SphereSlidersMap)
            {
                // SphereFactoryが無い時はcontinue
                if(m_SphereDataCreator.SphereFactories.TryGetValue(operation, out var sphereFactory) == false) continue;

                // スライダーの表示設定
                sphereSliders.RadiusRangeSlider.style.display = setRadiusIndividualToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
                
                // 個別に設定するToggleが変化したときのスライダーの表示設定
                setRadiusIndividualToggle.RegisterValueChangedCallback(evt =>
                {
                    // スライダーの表示設定
                    sphereSliders.RadiusRangeSlider.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                });
                bool isRadiusModeChanged = false;
                if(operation != SphereOperation.NORMAL)
                {
                    if(m_SphereDataCreator.SphereFactories.TryGetValue(SphereOperation.NORMAL, out var normalSphereFactory))
                    {
                        setRadiusIndividualToggle.RegisterValueChangedCallback(evt =>
                        {
                            if(evt.newValue) return;

                            isRadiusModeChanged = true;
                            Vector2 normalSphereRadiusRange = normalSphereFactory.RadiusRange.ToVector2();
                            sphereFactory.RadiusRange.Set(normalSphereRadiusRange);
                            isRadiusModeChanged = false;
                        });
                    }
                }

                bool isSetFromSharedRadiusRange = false;
                // 球の半径
                radiusRangeSlider.RegisterValueChangedCallback(evt => 
                {
                    if(evt.target != evt.currentTarget) return;
                    if(setRadiusIndividualToggle.value) return;
                    isSetFromSharedRadiusRange = true;
                    sphereFactory.RadiusRange.Set(evt.newValue);
                    isSetFromSharedRadiusRange = false;
                });

                // 球の種類ごとの半径
                sphereSliders.RadiusRangeSlider.RegisterValueChangedCallback(evt =>
                {
                    if(evt.target != evt.currentTarget) return;
                    if(setRadiusIndividualToggle.value == false) return;
                    sphereFactory.RadiusRange.Set(evt.newValue);
                });

                if(operation != SphereOperation.NORMAL)
                {
                    sphereFactory.RadiusRange.Subscribe(v =>
                    {
                        if(isSetFromSharedRadiusRange) return;
                        if(isRadiusModeChanged) return;
                        setRadiusIndividualToggle.value = true;
                    }).AddTo(this);
                }
                sphereFactory.RadiusRange.Subscribe(v =>
                {
                    sphereSliders.RadiusRangeSlider.SetValueWithoutNotify(v.ToVector2());
                }).AddTo(this);

                // 球の割合
                Slider weightSlider = sphereSliders.WeightSlider;
                weightSlider.SetValueWithoutNotify(MathF.Round(m_SphereDataCreator.WeightedSphereOperations.GetWeight(operation), 3, MidpointRounding.AwayFromZero));
                weightSlider.RegisterValueChangedCallback(evt =>
                {
                    if(evt.target != evt.currentTarget) return;
                    m_SphereDataCreator.WeightedSphereOperations.SetNormalizedWeight(operation, evt.newValue);
                });
                m_SphereDataCreator.WeightedSphereOperations.Subscribe(v =>
                {
                    weightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(operation), 3, MidpointRounding.AwayFromZero));
                }).AddTo(this);
            }

            // 演算対象の球
            foreach(var (operation, sphereSliders) in m_SphereSlidersMap)
            {
                if(sphereSliders is not OperatedSphereSliders operatedSphereSliders) continue;
                if(m_SphereDataCreator.SphereFactories.TryGetValue(operation, out var sphereFactory) == false) continue;
                if(sphereFactory is not OperatedSphereFactory operatedSphereFactory) continue;

                operatedSphereSliders.TargetRadiusRangeSlider.RegisterValueChangedCallback(evt =>
                {
                    if(evt.target != evt.currentTarget) return;
                    operatedSphereFactory.OperationTargetRadiusRange.Set(evt.newValue);
                });
                operatedSphereSliders.TargetCountRangeSlider.RegisterValueChangedCallback(evt =>
                {
                    if(evt.target != evt.currentTarget) return;
                    operatedSphereFactory.OperationTargetCountRange.Set((uint)evt.newValue.x, (uint)evt.newValue.y);
                });
                operatedSphereFactory.OperationTargetRadiusRange.Subscribe(v =>
                {
                    operatedSphereSliders.TargetRadiusRangeSlider.SetValueWithoutNotify(v.ToVector2());
                }).AddTo(this);
                operatedSphereFactory.OperationTargetCountRange.Subscribe(v =>
                {
                    operatedSphereSliders.TargetCountRangeSlider.SetValueWithoutNotify(v.ToVector2());
                }).AddTo(this);
            }
        }
    }
}
