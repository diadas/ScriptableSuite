
using ScriptableSuite.Variables;
using TMPro;
using UnityEngine;

namespace ScriptableSuite.Components.TextMeshPro
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SetTextByFloatScriptable : MonoBehaviour, IScriptableVariableListener<float>
    {
        [SerializeField] private FloatScriptable _floatScriptable;
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _floatScriptable.Subscribe(this, true);
        }

        public void OnChange(IScriptableVariable<float> variable)
        {
            _text.text = _floatScriptable.Value.ToString("F2");
        }
    }
}