﻿using System;
using System.Collections;
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

        [SerializeField] private float _transitionDuration = 0.25f;
        [SerializeField] private FloatScriptable _floatScriptable;
        [SerializeField] private List<SwitchEntry> _playAudioClipListSwitch;
        private PlayAudioClipList _currentClipList = null;
        private SwitchEntry _nextClipList = null;
        private SwitchEntry _lastClipList = null;

        public FloatScriptable GetScriptable()
        {
            return _floatScriptable;
        }

        private void Start()
        {
            _floatScriptable.Subscribe(this);
        }

        public void OnChange(IScriptableVariable<float> variable)
        {
            var nextClipList = GetNext();
            _nextClipList = nextClipList;
            if (_nextClipList != null && _nextClipList.PlayAudioClipList == _currentClipList)
            {
                if (_currentClipList != null)
                {
                    _currentClipList.Shedule = null;
                }
                return;
            }

            if (_currentClipList != null)
            {
                if (_nextClipList == null)
                {
                    StartCoroutine(Stop());
                } else if (_nextClipList.Transition == TransitionType.Cut)
                {
                    _currentClipList.Stop();
                    PlayNext();
                }
                else if (_nextClipList.Transition == TransitionType.Transition)
                {
                    Debug.Log(_currentClipList);
                    Debug.Log(_currentClipList.CurrentTime);
                    var position = _currentClipList.CurrentTime;
                    _currentClipList.FadeOut(_transitionDuration);
                    PlayNext();
                    position %= _currentClipList.Length;
                    _currentClipList.FadeIn(_transitionDuration, position);
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

            _lastClipList = nextClipList;
        }

        private IEnumerator Stop()
        {
            yield return new WaitForSeconds(2f);
            
            _currentClipList.FadeOut(_transitionDuration);
            _currentClipList = null;
        }

        private SwitchEntry GetNext()
        {
            for (var i = 0; i < _playAudioClipListSwitch.Count; i++)
            {
                if (_floatScriptable.Value < _playAudioClipListSwitch[i].Threshhold && i > 0 && _playAudioClipListSwitch[i-1].Threshhold <= _floatScriptable.Value)
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
            bool fromBelow = !(_lastClipList != null && _nextClipList != null && _nextClipList.Threshhold < _lastClipList.Threshhold);
            _currentClipList = _nextClipList.PlayAudioClipList;
            _currentClipList.Shedule = null;
            _currentClipList.Play(true, fromBelow);
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