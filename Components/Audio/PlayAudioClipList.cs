using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableSuite.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptableSuite.Components.Audio
{
    public class PlayAudioClipList : MonoBehaviour
    {
        public IPlayAudioClipListListener Shedule = null;

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                UpdateVolume();
            }
        }
        public float CurrentTime
        {
            get { return _currentAudioSource!=null ? _currentAudioSource.time : 0f; }
        }
        public float Length
        {
            get { return _audioClipList.ClipLength; }
        }

        [SerializeField] private AudioClipListScriptable _audioClipList;
        [SerializeField] private AudioClipListScriptable _upTransition;
        [SerializeField] private AudioClipListScriptable _downTransition;
        [SerializeField] private bool _autoStart;
        [SerializeField] private int _current = 0;
        [SerializeField] private bool _randomize;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _skip = 0f;
        [SerializeField] private AudioSource _currentAudioSource;

        private Coroutine _coroutine = null;
        private readonly List<AudioSource> _audioSources = new List<AudioSource>();


        private void Start()
        {
            if (_autoStart)
            {
                Play();
            }
        }

        public void Stop()
        {
            StopCoroutine(_coroutine);
            for (var i = 0; i < _audioSources.Count; i++)
            {
                _audioSources[i].Stop();
            }
        }

        public void FadeOut(float duration = 0.25f)
        {
            StopCoroutine(_coroutine);
            for (var i = 0; i < _audioSources.Count; i++)
            {
                var audioSource = _audioSources[i];
                if (audioSource.isPlaying)
                {
                    audioSource.DOKill();
                    audioSource.DOFade(0f, duration).OnComplete(() => { audioSource.Stop(); });
                }
            }
        }

        public void FadeIn(float duration = 0.25f, float position = 0f)
        {
            if (position > 0f && _currentAudioSource != null)
            {
                _currentAudioSource.time = position;
            }
            for (var i = 0; i < _audioSources.Count; i++)
            {
                var audioSource = _audioSources[i];
                if (audioSource.isPlaying)
                {
                    audioSource.volume = 0f;
                    audioSource.DOKill();
                    audioSource.DOFade(1f, duration);
                }
            }
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(SheduleNext());
        }

        public void Play(bool start = false, bool fromBelow = true)
        {
            if (Shedule != null)
            {
                Shedule.OnChange(this);
                Shedule = null;
                return;
            }

            if (_randomize)
            {
                _current = Random.Range(0, _audioClipList.Value.Count);
            }
            else
            {
                _current = (_current + 1) % _audioClipList.Value.Count;
            }

            if (start && (fromBelow && _upTransition != null || !fromBelow && _downTransition != null))
            {
                PlayTransition(fromBelow);
                return;
            }

            var audioSource = GetOrCreateAudioSource();
            audioSource.clip = _audioClipList.Value[_current];
            audioSource.volume = _volume;
            audioSource.Play();
            if (_skip > 0f)
            {
                audioSource.time = _skip;
            }
            _currentAudioSource = audioSource;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(SheduleNext());
        }

        private void PlayTransition(bool fromBelow)
        {
            var clipList = (fromBelow ? _upTransition : _downTransition);
            var audioSource = GetOrCreateAudioSource();
            audioSource.clip = clipList.Value[0];
            audioSource.volume = _volume;
            audioSource.Play();
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(SheduleNext(clipList));
        }

        private void UpdateVolume()
        {
            for (var i = 0; i < _audioSources.Count; i++)
            {
                if (_audioSources[i].isPlaying)
                {
                    _audioSources[i].volume = _volume;
                }
            }
        }

        private IEnumerator SheduleNext(AudioClipListScriptable clipList = null)
        {
            if (clipList == null)
            {
                clipList = _audioClipList;
            }
            yield return new WaitForSeconds(clipList.ClipLength-_skip-_currentAudioSource.time);
            Play();
        }

        private AudioSource GetOrCreateAudioSource()
        {
            for (var i = 0; i < _audioSources.Count; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    return _audioSources[i];
                }
            }

            var audioSourceGameObject = new GameObject("AudioSource" + _audioSources.Count);
            audioSourceGameObject.transform.parent = transform;
            var audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            _audioSources.Add(audioSource);
            return audioSource;
        }
    }
}