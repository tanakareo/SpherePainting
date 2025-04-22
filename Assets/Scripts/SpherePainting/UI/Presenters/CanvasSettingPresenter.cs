using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    using ShapeType = Canvas.ShapeType;

    [RequireComponent(typeof(UIDocument))]
    public class CanvasSettingPresenter : MonoBehaviour
    {
        private static readonly Dictionary<ShapeType, string> SHAPE_TYPE_CHOICE_NAMES
        = new (){
                    {ShapeType.ELLIPSE, "楕円"}, 
                    {ShapeType.RECTANGLE, "長方形"}
                };

        [SerializeField] private Canvas m_Canvas;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // 奥行き
            var canvasDepthUnitSlider = root.Q<UnitSlider>("canvas-depth-unit-slider");
            canvasDepthUnitSlider.lowValue = Canvas.MIN_DEPTH;
            canvasDepthUnitSlider.highValue = Canvas.MAX_DEPTH;
            canvasDepthUnitSlider.value = m_Canvas.Depth.CurrentValue;
            canvasDepthUnitSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_Canvas.SetDepth(v.newValue);
            });
            m_Canvas.Depth.Subscribe(v => canvasDepthUnitSlider.SetValueWithoutNotify(v)).AddTo(this);
            canvasDepthUnitSlider.UnitSuffix = "mm";
            canvasDepthUnitSlider.UnitToUnityUnit = UnitUtility.MILLIMETER_TO_UNITY_UNIT;

            // キャンバスの大きさ
            var widthUnitSlider = root.Q<UnitSlider>("canvas-width-unit-slider");
            widthUnitSlider.lowValue = Canvas.MIN_SIZE;
            widthUnitSlider.highValue = Canvas.MAX_SIZE;
            widthUnitSlider.value = m_Canvas.Size.CurrentValue.x;
            widthUnitSlider.UnitSuffix = "cm";
            widthUnitSlider.UnitToUnityUnit = UnitUtility.CENTIMETER_TO_UNITY_UNIT;

            var heightUnitSlider = root.Q<UnitSlider>("canvas-height-unit-slider");
            heightUnitSlider.lowValue = Canvas.MIN_SIZE;
            heightUnitSlider.highValue = Canvas.MAX_SIZE;
            heightUnitSlider.value = m_Canvas.Size.CurrentValue.y;
            heightUnitSlider.UnitSuffix = "cm";
            heightUnitSlider.UnitToUnityUnit = UnitUtility.CENTIMETER_TO_UNITY_UNIT;
            
            var preserveAspectRatioToggle = root.Q<Toggle>("canvas-preserve-aspect-ratio-toggle");
            preserveAspectRatioToggle.value = true;
            widthUnitSlider.RegisterValueChangedCallback(v =>
            {
                if (v.target != v.currentTarget) return;
                
                float newWidth = v.newValue;
                float newHeight = heightUnitSlider.value;

                if (preserveAspectRatioToggle.value)
                {
                    newHeight = newWidth / (v.previousValue / newHeight);
                    
                    // newHeight がスライダーの最大値を超えないように調整
                    if (newHeight > heightUnitSlider.highValue)
                    {
                        newHeight = heightUnitSlider.highValue;
                        newWidth = newHeight * (v.previousValue / heightUnitSlider.value);
                    }
                }

                m_Canvas.SetSize(new Vector2(newWidth, newHeight));
            });

            heightUnitSlider.RegisterValueChangedCallback(v =>
            {
                if (v.target != v.currentTarget) return;
                
                float newHeight = v.newValue;
                float newWidth = widthUnitSlider.value;

                if (preserveAspectRatioToggle.value)
                {
                    newWidth = newHeight / (v.previousValue / newWidth);

                    // newWidth がスライダーの最大値を超えないように調整
                    if (newWidth > widthUnitSlider.highValue)
                    {
                        newWidth = widthUnitSlider.highValue;
                        newHeight = newWidth * (v.previousValue / widthUnitSlider.value);
                    }
                }

                m_Canvas.SetSize(new Vector2(newWidth, newHeight));
            });

            m_Canvas.Size.Subscribe(v =>
            {
                widthUnitSlider.SetValueWithoutNotify(v.x);
                heightUnitSlider.SetValueWithoutNotify(v.y);
            }).AddTo(this);
            
            // キャンバスの形
            var shapeTypeDropdownField = root.Q<DropdownField>("canvas-shape-type-dropdown-field");
            shapeTypeDropdownField.choices = SHAPE_TYPE_CHOICE_NAMES.Values.ToList();
            shapeTypeDropdownField.value = SHAPE_TYPE_CHOICE_NAMES[m_Canvas.CurrentShapeType.CurrentValue];
            shapeTypeDropdownField.RegisterValueChangedCallback(evt =>
            {
                ShapeType shapeType = SHAPE_TYPE_CHOICE_NAMES.Where(v => evt.newValue == v.Value).Select(v => v.Key).First();
                m_Canvas.ChangeCanvasShapeType(shapeType);
            });
            m_Canvas.CurrentShapeType.Subscribe(v => shapeTypeDropdownField.SetValueWithoutNotify(SHAPE_TYPE_CHOICE_NAMES[v])).AddTo(this);
            // マッピング
            var horizontalTextureOffsetSlider = root.Q<Slider>("canvas-texture-horizontal-offset-slider");
            horizontalTextureOffsetSlider.value = m_Canvas.NormalizedTextureOffset.CurrentValue.x;
            var verticalTextureOffsetSlider = root.Q<Slider>("canvas-texture-vertical-offset-slider");
            verticalTextureOffsetSlider.value = m_Canvas.NormalizedTextureOffset.CurrentValue.x;
            horizontalTextureOffsetSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_Canvas.SetTextureOffset(new Vector2(v.newValue, verticalTextureOffsetSlider.value));
            });
            verticalTextureOffsetSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_Canvas.SetTextureOffset(new Vector2(horizontalTextureOffsetSlider.value, v.newValue));
            });
            m_Canvas.NormalizedTextureOffset.Subscribe(v => 
            {
                horizontalTextureOffsetSlider.SetValueWithoutNotify(v.x);
                verticalTextureOffsetSlider.SetValueWithoutNotify(v.y);
            }).AddTo(this);

            var textureScaleSlider = root.Q<Slider>("canvas-texture-scale-slider");
            textureScaleSlider.value = m_Canvas.TextureScaler.CurrentValue;
            textureScaleSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                m_Canvas.SetTextureScale(1.0f / v.newValue);
            });
            m_Canvas.TextureScaler.Subscribe(v => 
            {
                textureScaleSlider.SetValueWithoutNotify(1.0f / v);
            }).AddTo(this);

            // フィルター
            var layerShuffleToggle = root.Q<Toggle>("canvas-layer-shuffle-toggle");
            var layerShuffleSeedSlider = root.Q<SliderInt>("canvas-layer-shuffle-seed-slider-int");
            layerShuffleToggle.value = m_Canvas.IsLayerShuffleActive.CurrentValue;
            layerShuffleSeedSlider.style.display = layerShuffleToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
            layerShuffleToggle.RegisterValueChangedCallback(v =>
            {
                layerShuffleSeedSlider.style.display = layerShuffleToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
                if(v.newValue)
                {
                    m_Canvas.ShuffleLayer((uint)layerShuffleSeedSlider.value);
                }
                else
                {
                    m_Canvas.UnshuffleLayer();
                }
            });
            layerShuffleSeedSlider.RegisterValueChangedCallback(v =>
            {
                m_Canvas.ShuffleLayer((uint)v.newValue);
            });
            m_Canvas.IsLayerShuffleActive.Subscribe(v => layerShuffleToggle.value = v).AddTo(this);
            m_Canvas.LayerShuffleSeed.Subscribe(v => layerShuffleSeedSlider.SetValueWithoutNotify((int)v)).AddTo(this);

            var layerFilteringToggle = root.Q<Toggle>("layer-filtering-toggle");
            layerFilteringToggle.value = m_Canvas.IsLayerFilterActive.CurrentValue;
            
            var activeLayerIndexSlider = root.Q<SliderInt>("active-filtered-layer-index-slider-int");
            activeLayerIndexSlider.style.display = layerFilteringToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
            layerFilteringToggle.RegisterValueChangedCallback(v =>
            {
                activeLayerIndexSlider.style.display = v.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                if(v.newValue)
                {
                    m_Canvas.FilterLayer(activeLayerIndexSlider.value - 1);
                }
                else
                {
                    m_Canvas.ShowActiveLayers();
                }
            });
            activeLayerIndexSlider.lowValue = 1;
            m_Canvas.LayerCount.Subscribe(v =>
            {
                activeLayerIndexSlider.highValue = v;
            }).AddTo(this);
            activeLayerIndexSlider.RegisterValueChangedCallback(v =>
            {
                if(v.target != v.currentTarget) return;
                if(layerFilteringToggle.value == false) return;
                m_Canvas.FilterLayer(v.newValue - 1);
            });
            m_Canvas.ActiveFilteredLayer.Subscribe(v => activeLayerIndexSlider.SetValueWithoutNotify(v + 1)).AddTo(this);
            m_Canvas.IsLayerFilterActive.Subscribe(v => layerFilteringToggle.value = v ).AddTo(this);
        }
    }
}
