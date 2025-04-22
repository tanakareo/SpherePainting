using UnityEngine;
using UnityEngine.UIElements;

namespace SpherePainting
{
    [UxmlElement("MinMaxSlider")]
    public partial class MinMaxSlider : UnityEngine.UIElements.MinMaxSlider
    {
        [UxmlAttribute] private bool m_IsInteger = false;
        private FloatField m_MinFloatField, m_MaxFloatField;
        public FloatField MinFloatField
        {
            get
            {
                if(m_MinFloatField == null)
                {
                    m_MinFloatField = CreateCustomFloatField();
                }
                return m_MinFloatField;
            }
        }

        public FloatField MaxFloatField
        {
            get
            {
                if(m_MaxFloatField == null)
                {
                    m_MaxFloatField = CreateCustomFloatField();
                }
                return m_MaxFloatField;
            }
        }
        
        public MinMaxSlider()
        {
            // MinMaxSliderの値が変更された時にFloatFieldに反映
            this.RegisterValueChangedCallback(evt =>
            {
                float newMin = evt.newValue.x;
                float newMax = evt.newValue.y;
                if(m_IsInteger)
                {
                    newMin = (int)newMin;
                    newMax = (int)newMax;
                }
                
                MinFloatField.value = newMin;
                MaxFloatField.value = newMax;
            });

            // `unity-min-max-slider__input` クラスの要素を取得
            var minMaxSliderInput = this.Q<VisualElement>(className: "unity-min-max-slider__input");
            var inputContainer = new VisualElement();
            if (minMaxSliderInput != null)
            {
                inputContainer.AddToClassList("min-max-slider__input");
                inputContainer.style.flexDirection = FlexDirection.Row;
                inputContainer.style.flexGrow = 1;
                inputContainer.style.flexShrink = 0;
                inputContainer.Add(MinFloatField);
                inputContainer.Add(minMaxSliderInput);
                inputContainer.Add(MaxFloatField);
                Add(inputContainer);
            }
            else
            {
                Debug.LogWarning("unity-min-max-slider__input class not found!");
            }
            
            RegisterCallback<AttachToPanelEvent>(e =>
            {
                MinFloatField.value = minValue;
                MaxFloatField.value = maxValue;

                MinFloatField.style.marginRight = 10;
                MinFloatField.style.marginTop = 0;
                MinFloatField.style.marginLeft = 0;
                MinFloatField.style.marginBottom = 0;
                MaxFloatField.style.marginLeft = 10;
                MaxFloatField.style.marginTop = 0;
                MaxFloatField.style.marginRight = 0;
                MaxFloatField.style.marginBottom = 0;

                MinFloatField.RegisterCallback<FocusOutEvent>(evt => 
                {
                    value = new Vector2(MinFloatField.value, Mathf.Max(MinFloatField.value, value.y));
                });
                MaxFloatField.RegisterCallback<FocusOutEvent>(evt => 
                {
                    value = new Vector2(Mathf.Min(value.x, MaxFloatField.value), MaxFloatField.value);
                });
            });
        }

        // カスタムFloatFieldを作成
        private FloatField CreateCustomFloatField()
        {
            var floatField = new FloatField() {};
            
            // 値が変更されたときにフォーマットを適用
            floatField.RegisterValueChangedCallback(evt =>
            {
                if (m_IsInteger || evt.newValue % 1 == 0) // 小数点以下が0か確認
                {
                    floatField.formatString = "";
                    floatField.SetValueWithoutNotify((int)evt.newValue); // 整数として表示
                }
                else
                {
                    floatField.formatString = "F1";
                    floatField.SetValueWithoutNotify(evt.newValue); // 小数として表示
                }
            });

            return floatField;
        }

        public override void SetValueWithoutNotify(Vector2 newValue)
        {
            base.SetValueWithoutNotify(newValue);
            MinFloatField.value = newValue.x;
            MaxFloatField.value = newValue.y;
        }
    }
}