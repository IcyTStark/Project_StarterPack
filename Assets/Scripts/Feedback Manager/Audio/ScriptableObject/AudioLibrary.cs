using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "ScriptableObjects/Parameters/AudioLibrary")]
public class AudioLibrary : SerializedScriptableObject
{
    [DictionaryDrawerSettings]
    [SerializeField] private Dictionary<AudioType, AudioClip> _audioClips = new Dictionary<AudioType, AudioClip>();
}