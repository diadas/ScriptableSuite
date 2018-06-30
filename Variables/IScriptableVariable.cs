using System.Collections.Generic;
using UnityEngine;

namespace ScriptableSuite.Variables
{
	public class IScriptableVariable<T> : ScriptableObject
	{
		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				NotifySubscribers();
			}
		}
		private T _value;
		private List<IScriptableVariableListener<T>> _listeners;
		[SerializeField] private T _defaultValue;

		public void Subscribe(IScriptableVariableListener<T> listener, bool callAfterwards = false)
		{
			_listeners.Add(listener);
			if (callAfterwards)
			{
				listener.OnChange(this);
			}
		}

		public void NotifySubscribers()
		{
			for (var i = 0; i < _listeners.Count; i++)
			{
				_listeners[i].OnChange(this);
			}
		}
		
		private void OnEnable()
		{
			_listeners = new List<IScriptableVariableListener<T>>();
			Value = _defaultValue;
		}
	}
}
