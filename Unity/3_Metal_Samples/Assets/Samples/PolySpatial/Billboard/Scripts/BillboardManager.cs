using System;
using TMPro;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.UI;

namespace PolySpatial.Samples
{
    public class BillboardManager : MonoBehaviour
    {
        [SerializeField]
        VisionOSBillboard m_Billboard;

        [SerializeField]
        TMP_Text m_EnabledText;

        [SerializeField]
        TMP_Text m_BlendFactorText;

        [SerializeField]
        TMP_Text m_SliderTextValue;

        [SerializeField]
        Slider m_Slider;

        bool m_Enabled = true;

        void Start()
        {
            m_BlendFactorText.text = m_Billboard.BlendFactor.ToString("F2");
        }

        public void EnabledToggle()
        {
            m_Enabled = !m_Enabled;
            m_EnabledText.text = m_Enabled ? "Enabled" : "Disabled";
        }

        void Update()
        {
            var sliderValue = m_Slider.value.ToString("F2");
            m_BlendFactorText.text = sliderValue;
            m_SliderTextValue.text = sliderValue;
        }
    }
}
