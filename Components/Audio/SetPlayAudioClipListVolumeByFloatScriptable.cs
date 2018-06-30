using ScriptableSuite.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableSuite.Components.TextMeshPro
{
    [RequireComponent(typeof(PlayAudioClipList))]
    public class SetPlayAudioClipListVolumeByFloatScriptable : MonoBehaviour, IScriptableVariableListener<float>
    {
        public Vector2 FadeOut
        {
            get { return _fadeOut; }
        }
        [SerializeField] private FloatScriptable _floatScriptable;
        [SerializeField, MinMaxSlider(0f,1f)] private Vector2 _volumes;
        [SerializeField] private SetPlayAudioClipListVolumeByFloatScriptable _predecessor;
        [SerializeField, MinMaxSlider(0f,1f), HideIf("HasPredecessor")] private Vector2 _fadeIn;
        [SerializeField, MinMaxSlider(0f,1f)] private Vector2 _fadeOut;
        private PlayAudioClipList _playAudioClipList;

        private void Awake()
        {
            if (HasPredecessor())
            {
                _fadeIn = _predecessor.FadeOut;
            }
        }
        
        private void Start()
        {
            _playAudioClipList = GetComponent<PlayAudioClipList>();
            _floatScriptable.Subscribe(this, true);
        }

        public void OnChange(IScriptableVariable<float> variable)
        {
            var input = Mathf.Clamp01(_floatScriptable.Value);
            float volume = 0f;
            if (input > _fadeIn.x && input < _fadeIn.y)
            {
                volume = (input-_fadeIn.x)/_fadeIn.y;
            } else if (input >= _fadeIn.y && input <= _fadeOut.x)
            {
                volume = 1f;
            } else if (input > _fadeOut.x && input < _fadeOut.y)
            {
                volume = 1f-(input-_fadeOut.x)/_fadeOut.y;
            }
            
            _playAudioClipList.Volume = volume;
        }

        private bool HasPredecessor()
        {
            return _predecessor != null;
        }
    }
}