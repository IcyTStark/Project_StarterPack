using UnityEngine;

namespace gamedev.AudioManager
{
    public interface IAudioService
    {
        void Play(AudioClip clip);
        void Stop();
        void Pause();
        void Resume();
        void SetVolume(float volume);
        bool IsPlaying { get; }
    }
}
