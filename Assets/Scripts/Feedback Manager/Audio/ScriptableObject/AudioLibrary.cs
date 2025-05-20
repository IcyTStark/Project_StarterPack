using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "ScriptableObjects/Parameters/AudioLibrary")]
public class AudioLibrary : SerializedScriptableObject
{
    [DictionaryDrawerSettings]
    [SerializeField] private Dictionary<AudioType, AudioClip> _audioClips = new Dictionary<AudioType, AudioClip>();

    /// <summary>
    /// Gets an audio clip by its type
    /// </summary>
    /// <param name="audioType">The type of audio to retrieve</param>
    /// <returns>The AudioClip associated with the type, or null if not found</returns>
    public AudioClip GetAudioClip(AudioType audioType)
    {
        if (_audioClips.TryGetValue(audioType, out AudioClip clip))
        {
            return clip;
        }

        Debug.LogWarning($"[AudioLibrary] No audio clip found for type: {audioType}");
        return null;
    }

    /// <summary>
    /// Checks if an audio clip exists for the specified type
    /// </summary>
    /// <param name="audioType">The type to check</param>
    /// <returns>True if a clip exists, false otherwise</returns>
    public bool HasAudioClip(AudioType audioType)
    {
        return _audioClips.ContainsKey(audioType);
    }

    /// <summary>
    /// Adds or updates an audio clip in the library
    /// </summary>
    /// <param name="audioType">The type of audio</param>
    /// <param name="clip">The audio clip to associate with the type</param>
    public void SetAudioClip(AudioType audioType, AudioClip clip)
    {
        _audioClips[audioType] = clip;
    }

    /// <summary>
    /// Gets all registered audio types in the library
    /// </summary>
    public IEnumerable<AudioType> GetAllAudioTypes()
    {
        return _audioClips.Keys;
    }
}