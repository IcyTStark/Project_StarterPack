using UnityEngine;

namespace TMS.Feedback.Audio
{
    public interface IAudio
    {
        bool IsPlaying { get; }

        void ToggleMusic(bool musicState);

        void ToggleSFX(bool sfxState);

        void Play(AudioClip clip);

        void PlayDelayed(AudioClip clip, float delayTime);

        void PlayOneShot(AudioClip clip, float volumeScale = 1);

        void PlayClipAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1);

        void PlayScheduled(float scheduledTime);

        void Stop();

        void Pause();

        void Resume();

        void SetVolume(float volume);
    }
}
