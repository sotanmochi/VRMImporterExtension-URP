using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRM.Extension.Samples
{
    public class ShaderSelectView : MonoBehaviour, IShaderSelectEventTrigger
    {
        [SerializeField] Dropdown _ShaderSelector;

        public event Action<string> OnTriggerShaderSelectEvent;

        void Awake()
        {
            _ShaderSelector.onValueChanged.AddListener(index => 
            {
                string shaderType = _ShaderSelector.options[index].text;
                OnTriggerShaderSelectEvent?.Invoke(shaderType);
            });
        }
    }
}
