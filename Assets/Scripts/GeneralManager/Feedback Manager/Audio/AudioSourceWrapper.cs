using UnityEngine;

namespace gamedev.AudioManager
{
    public class AudioSourceWrapper : IAudioService
    {
        private AudioSource audioSource;

        public AudioSourceWrapper(AudioSource source)
        {
            audioSource = source;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            audioSource.PlayOneShot(clip, volume);
        }

        public void Stop() => audioSource.Stop();
        public void Pause() => audioSource.Pause();
        public void Resume() => audioSource.UnPause();
        public void SetVolume(float volume) => audioSource.volume = volume;
        public bool IsPlaying => audioSource.isPlaying;

        public AudioClip CurrentClip => audioSource.clip;
    }
}
