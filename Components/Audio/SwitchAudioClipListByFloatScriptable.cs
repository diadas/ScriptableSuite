using System;
using System.Collections.Generic;
using ScriptableSuite.Variables;
using UnityEngine;

namespace ScriptableSuite.Components.Audio
{
    public class SwitchAudioClipListByFloatScriptable : MonoBehaviour, IScriptableVariableListener<float>, IPlayAudioClipListListener
    {
        public enum TransitionType
        {
            Wait,
            Transition,
            Cut
        }
        [Serializable]
        public class SwitchEntry
        {
            public PlayAudioClipList PlayAudioClipList;
            public float Threshhold;
            public TransitionType Transition;
        }
        [SerializeField] private FloatScriptable _floatScriptable;
        [SerializeField] private List<SwitchEntry> _playAudioClipListSwitch;
        private PlayAudioClipList _currentClipList = null;
        private SwitchEntry _nextClipList = null;

        private void Start()
        {
            _floatScriptable.Subscribe(this);
        }

        public void OnChange(IScriptableVariable<float> variable)
        {
            _nextClipList = GetNext();
            if (_nextClipList.PlayAudioClipList == _currentClipList)
            {
                if (_currentClipList != null)
                {
                    _currentClipList.Shedule = null;
                }
                return;
            }

            if (_currentClipList != null)
            {
                if (_nextClipList.Transition == TransitionType.Cut)
                {
                    _currentClipList.Stop();
                    PlayNext();
                }
                else
                {
                    _currentClipList.Shedule = this;
                }
            }
            else
            {
                PlayNext();
            }
        }

        private SwitchEntry GetNext()
        {
            for (var i = 0; i < _playAudioClipListSwitch.Count; i++)
            {
                if (_floatScriptable.Value < _playAudioClipListSwitch[i].Threshhold && i > 0)
                {
                    return _playAudioClipListSwitch[i-1];
                }
            }

            var last = _playAudioClipListSwitch[_playAudioClipListSwitch.Count - 1];
            if (_floatScriptable.Value >= last.Threshhold)
            {
                return last;
            }

            return null;
        }

        private void PlayNext()
        {
            _currentClipList = _nextClipList.PlayAudioClipList;
            _currentClipList.Shedule = null;
            _currentClipList.Play();
            _nextClipList = null;
        }

        public void OnChange(PlayAudioClipList variable)
        {
            if (_nextClipList != null)
            {
                PlayNext();
            }
        }
    }
}