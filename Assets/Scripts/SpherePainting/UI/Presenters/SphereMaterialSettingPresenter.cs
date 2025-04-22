using System;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using Range;
using WeightedValues;

namespace SpherePainting
{
    public class SphereMaterialSettingPresenter : MonoBehaviour
    {
        private static readonly int SLIDER_COLOR_RANGE_MAX = 255;

        [SerializeField] private SphereMaterialDataCreator m_SphereMaterialDataCreator;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var seedSlider = root.Q<SliderInt>("sphere-material-seed-slider-int");
            var hueRangeSlider = root.Q<MinMaxSlider>("material-color-hue-range-min-max-slider");
            var saturationRangeSlider = root.Q<MinMaxSlider>("material-color-saturation-range-min-max-slider");
            var valueRangeSlider = root.Q<MinMaxSlider>("material-color-value-range-min-max-slider");
            var opacityRangeSlider = root.Q<MinMaxSlider>("material-color-opacity-range-min-max-slider");

            var operationTargetOpacityRangeSlider = root.Q<MinMaxSlider>("operation-target-material-color-opacity-range-min-max-slider");
            var transparentOperationTargetRatioSlider = root.Q<Slider>("transparent-operation-target-ratio-slider");  
            var operationTargetMaterialRandomnessRangeSlider = root.Q<MinMaxSlider>("operation-target-material-randomness-range-min-max-slider");
            var operationAreaMaterialRandomnessRangeSlider = root.Q<MinMaxSlider>("operation-area-material-randomness-range-min-max-slider");

            seedSlider.value = (int)m_SphereMaterialDataCreator.Seed.CurrentValue;
            
            hueRangeSlider.value = m_SphereMaterialDataCreator.MaterialColorRange.HueRange.ToVector2() * SLIDER_COLOR_RANGE_MAX;
            saturationRangeSlider.value = m_SphereMaterialDataCreator.MaterialColorRange.SaturationRange.ToVector2() * SLIDER_COLOR_RANGE_MAX;
            valueRangeSlider.value = m_SphereMaterialDataCreator.MaterialColorRange.ValueRange.ToVector2() * SLIDER_COLOR_RANGE_MAX;
            opacityRangeSlider.value = m_SphereMaterialDataCreator.MaterialColorRange.OpacityRange.ToVector2() * SLIDER_COLOR_RANGE_MAX;
            operationTargetOpacityRangeSlider.value = m_SphereMaterialDataCreator.OperationTargetMaterialOpacityRange.ToVector2() * SLIDER_COLOR_RANGE_MAX;
            transparentOperationTargetRatioSlider.value = m_SphereMaterialDataCreator.TransparentOperationTargetRatio.CurrentValue;

            operationTargetMaterialRandomnessRangeSlider.lowLimit = 0.0f;
            operationTargetMaterialRandomnessRangeSlider.highLimit = 1.0f;
            operationTargetMaterialRandomnessRangeSlider.value = m_SphereMaterialDataCreator.OperationTargetMaterialRandomnessRange.ToVector2();
           
            operationAreaMaterialRandomnessRangeSlider.lowLimit = 0.0f;
            operationAreaMaterialRandomnessRangeSlider.highLimit = 1.0f;
            operationAreaMaterialRandomnessRangeSlider.value = m_SphereMaterialDataCreator.OperationAreaMaterialRandomnessRange.ToVector2();

            seedSlider.RegisterValueChangedCallback(evt => 
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.SetSeed((uint)evt.newValue);
            });

            hueRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.MaterialColorRange.HueRange.Set(evt.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            saturationRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.MaterialColorRange.SaturationRange.Set(evt.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            valueRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.MaterialColorRange.ValueRange.Set(evt.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            opacityRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.MaterialColorRange.OpacityRange.Set(evt.newValue / SLIDER_COLOR_RANGE_MAX);
            });
            operationTargetOpacityRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.OperationTargetMaterialOpacityRange.Set(evt.newValue / SLIDER_COLOR_RANGE_MAX);
            });

            transparentOperationTargetRatioSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.SetTransparentOperationTargetRatio(evt.newValue);
            });

            operationAreaMaterialRandomnessRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.OperationAreaMaterialRandomnessRange.Set(evt.newValue);
            });

            operationTargetMaterialRandomnessRangeSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.OperationTargetMaterialRandomnessRange.Set(evt.newValue);
            });

            m_SphereMaterialDataCreator.OperationAreaMaterialRandomnessRange.Subscribe(v =>
            {
                operationAreaMaterialRandomnessRangeSlider.SetValueWithoutNotify(v.ToVector2());
            }).AddTo(this);
            m_SphereMaterialDataCreator.OperationTargetMaterialRandomnessRange.Subscribe(v =>
            {
                operationTargetMaterialRandomnessRangeSlider.SetValueWithoutNotify(v.ToVector2());
            }).AddTo(this);

            m_SphereMaterialDataCreator.Seed.Subscribe(v =>
            {
                seedSlider.SetValueWithoutNotify((int)v);
            }).AddTo(this);
            m_SphereMaterialDataCreator.MaterialColorRange.HueRange.Subscribe(v => { hueRangeSlider.SetValueWithoutNotify(v.ToVector2() * SLIDER_COLOR_RANGE_MAX); }).AddTo(this);
            m_SphereMaterialDataCreator.MaterialColorRange.SaturationRange.Subscribe(v => { saturationRangeSlider.SetValueWithoutNotify(v.ToVector2() * SLIDER_COLOR_RANGE_MAX); }).AddTo(this);
            m_SphereMaterialDataCreator.MaterialColorRange.ValueRange.Subscribe(v => { valueRangeSlider.SetValueWithoutNotify(v.ToVector2() * SLIDER_COLOR_RANGE_MAX); }).AddTo(this);
            m_SphereMaterialDataCreator.MaterialColorRange.OpacityRange.Subscribe(v => { opacityRangeSlider.SetValueWithoutNotify(v.ToVector2() * SLIDER_COLOR_RANGE_MAX); }).AddTo(this);
            m_SphereMaterialDataCreator.OperationTargetMaterialOpacityRange.Subscribe(v => { operationTargetOpacityRangeSlider.SetValueWithoutNotify(v.ToVector2() * SLIDER_COLOR_RANGE_MAX); }).AddTo(this);
            m_SphereMaterialDataCreator.TransparentOperationTargetRatio.Subscribe(v => transparentOperationTargetRatioSlider.SetValueWithoutNotify(v)).AddTo(this);
            
            // 合成モードの割合
            var normalBlendModeWeightSlider = root.Q<Slider>("normal-blend-mode-weight-slider");
            var multiplyBlendModeWeightSlider = root.Q<Slider>("multiply-blend-mode-weight-slider");
            var overlayBlendModeWeightSlider = root.Q<Slider>("overlay-blend-mode-weight-slider");
            var addBlendModeWeightSlider = root.Q<Slider>("add-blend-mode-weight-slider");
            var addGlowBlendModeWeightSlider = root.Q<Slider>("add-glow-blend-mode-weight-slider");
            var differenceBlendModeWeightSlider = root.Q<Slider>("difference-blend-mode-weight-slider");
            var exclusionBlendModeWeightSlider = root.Q<Slider>("exclusion-blend-mode-weight-slider");
            var hueBlendModeWeightSlider = root.Q<Slider>("hue-blend-mode-weight-slider");
            var saturationBlendModeWeightSlider = root.Q<Slider>("saturation-blend-mode-weight-slider");
            var colorBlendModeWeightSlider = root.Q<Slider>("color-blend-mode-weight-slider");
            var luminosityBlendModeWeightSlider = root.Q<Slider>("luminosity-blend-mode-weight-slider");
            normalBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.NORMAL), 3, MidpointRounding.AwayFromZero);
            multiplyBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.MULTIPLY), 3, MidpointRounding.AwayFromZero);
            overlayBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.OVERLAY), 3, MidpointRounding.AwayFromZero);
            addBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.ADD), 3, MidpointRounding.AwayFromZero);
            addGlowBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.ADD_GLOW), 3, MidpointRounding.AwayFromZero);
            differenceBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.DIFFERENCE), 3, MidpointRounding.AwayFromZero);
            exclusionBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.EXCLUSION), 3, MidpointRounding.AwayFromZero);
            hueBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.HUE), 3, MidpointRounding.AwayFromZero);
            saturationBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.SATURATION), 3, MidpointRounding.AwayFromZero);
            colorBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.COLOR), 3, MidpointRounding.AwayFromZero);
            luminosityBlendModeWeightSlider.value = MathF.Round(m_SphereMaterialDataCreator.WeightedBlendModes.GetWeight(BlendMode.LUMINOSITY), 3, MidpointRounding.AwayFromZero);
            normalBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.NORMAL, evt.newValue);
            });
            multiplyBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.MULTIPLY, evt.newValue);
            });
            overlayBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.OVERLAY, evt.newValue);
            });
            addBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.ADD, evt.newValue);
            });
            addGlowBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.ADD_GLOW, evt.newValue);
            });
            differenceBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.DIFFERENCE, evt.newValue);
            });
            exclusionBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.EXCLUSION, evt.newValue);
            });
            hueBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.HUE, evt.newValue);
            });
            saturationBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.SATURATION, evt.newValue);
            });
            colorBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.COLOR, evt.newValue);
            });
            luminosityBlendModeWeightSlider.RegisterValueChangedCallback(evt =>
            {
                if(evt.target != evt.currentTarget) return;
                m_SphereMaterialDataCreator.WeightedBlendModes.SetNormalizedWeight(BlendMode.LUMINOSITY, evt.newValue);
            });
            m_SphereMaterialDataCreator.WeightedBlendModes.Subscribe(v =>
            {
                normalBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.NORMAL), 3, MidpointRounding.AwayFromZero));
                multiplyBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.MULTIPLY), 3, MidpointRounding.AwayFromZero));
                overlayBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.OVERLAY), 3, MidpointRounding.AwayFromZero));
                addBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.ADD), 3, MidpointRounding.AwayFromZero));
                addGlowBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.ADD_GLOW), 3, MidpointRounding.AwayFromZero));
                differenceBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.DIFFERENCE), 3, MidpointRounding.AwayFromZero));
                exclusionBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.EXCLUSION), 3, MidpointRounding.AwayFromZero));
                hueBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.HUE), 3, MidpointRounding.AwayFromZero));
                saturationBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.SATURATION), 3, MidpointRounding.AwayFromZero));
                colorBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.COLOR), 3, MidpointRounding.AwayFromZero));
                luminosityBlendModeWeightSlider.SetValueWithoutNotify(MathF.Round(v.GetWeight(BlendMode.LUMINOSITY), 3, MidpointRounding.AwayFromZero));
            }).AddTo(this);
        }
    }
}
