using System;
using UnityEngine;

namespace ScriptableSuite.Variables
{
	public class IScriptableVariable<T> : ScriptableObject
	{
		[NonSerialized] public T Value;
		[SerializeField] private T _defaultValue;

		private void OnEnable()
		{
			Value = _defaultValue;
		}
	}
}
