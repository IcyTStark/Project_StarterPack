using UnityEngine;
using Sirenix.OdinInspector;
using TMS.Feedback.Audio;
using UnityEngine.Pool;

public class AudioSourceItem : MonoBehaviour
{
    [BoxGroup]
    [SerializeField] private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;

    private AudioSourceSFXPool _audioSourcePool;

    public AudioSourceSFXPool AudioSourcePool { set => _audioSourcePool = value; }

    public void Initialize(AudioSettingsConfigSO audioSettings)
    {
        //Fail Safe Check
        _audioSource ??= GetComponent<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.volume = audioSettings.MasterVolume * audioSettings.SfxVolume; // Set default volume
    }

    private void ReleaseObject()
    {
        _audioSourcePool.ReturnObject(this);
    }
}
