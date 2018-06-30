using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private AudioClipListScriptable _audioClipList;
        [SerializeField] private bool _autoStart;
        [SerializeField] private int _current = 0;
        [SerializeField] private bool _randomize;
        [SerializeField] private float _volume = 1f;

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

        public void Play()
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
            
            var audioSource = GetOrCreateAudioSource();
            audioSource.clip = _audioClipList.Value[_current];
            audioSource.volume = _volume;
            audioSource.Play();
            _coroutine = StartCoroutine(SheduleNext());
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

        private IEnumerator SheduleNext()
        {
            yield return new WaitForSeconds(_audioClipList.ClipLength);
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
            var audioSourceGameObject = new GameObject("AudioSource"+_audioSources.Count);
            audioSourceGameObject.transform.parent = transform;
            var audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            _audioSources.Add(audioSource);
            return audioSource;
        }
    }
}