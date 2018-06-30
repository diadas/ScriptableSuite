using System;
using System.Collections.Generic;
using ScriptableSuite.Variables;
using UnityEngine;

namespace ScriptableSuite.Components.TextMeshPro
{
    [RequireComponent(typeof(PlayAudioClipList))]
    public class SwitchAudioClipListByFloatScriptable : MonoBehaviour, IScriptableVariableListener<float>
    {
        [Serializable]
        public struct SwitchEntry
        {
            public PlayAudioClipList PlayAudioClipList;
            public float Threshhold;
        }
        [SerializeField] private FloatScriptable _floatScriptable;
        [SerializeField] private List<SwitchEntry> _playAudioClipListSwitch;

        private void Start()
        {
            _floatScriptable.Subscribe(this);
        }

        public void OnChange(IScriptableVariable<float> variable)
        {
            for (var i = 0; i < _playAudioClipListSwitch.Count; i++)
            {
                if (_playAudioClipListSwitch[i].Threshhold < _floatScriptable.Value)
                {
                    
                }
            }
        }

        private PlayAudioClipList GetNext()
        {
            for (var i = 0; i < _playAudioClipListSwitch.Count; i++)
            {
                if (_playAudioClipListSwitch[i].Threshhold < _floatScriptable.Value && i > 0)
                {
                    return _playAudioClipListSwitch[i].PlayAudioClipList;
                }
            }

            return null;
        }
    }
}