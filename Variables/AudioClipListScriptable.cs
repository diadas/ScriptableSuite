using System.Collections.Generic;
using UnityEngine;

namespace ScriptableSuite.Variables
{
	[CreateAssetMenu(menuName = "ScriptableSuite/Variables/List<AudioClip>")]
	public class AudioClipListScriptable : IScriptableVariable<List<AudioClip>>
	{
		public float ClipLength;
	}
}
