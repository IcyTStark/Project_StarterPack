using gamedev.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private AudioSettings audioSettings;

    private Dictionary<string, IAudioService> sfxPlayers = new Dictionary<string, IAudioService>();
    private IAudioService musicPlayer;
    private IAudioService uiSoundPlayer;

    // Object Pooling for AudioSources
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private int poolSize = 10;

    public AudioManager()
    {

    }

    public AudioManager(AudioSettings inAudioSettings)
    {
        audioSettings = inAudioSettings;

        musicPlayer = CreateAudioService("MusicPlayer", true);
        uiSoundPlayer = CreateAudioService("UISoundPlayer");

        if (audioSettings.sfxClips.Length > 0)
        {
            foreach (AudioClip clip in audioSettings.sfxClips)
            {
                if (!sfxPlayers.ContainsKey(clip.name))
                    sfxPlayers[clip.name] = CreateAudioService(clip.name);
            }
        }

        InitializeAudioPool();
        ApplyVolumeSettings();
    }

    private void InitializeAudioPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"PooledAudioSource_{i}");
            obj.transform.SetParent(audioSettings.audioHolder);
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            obj.SetActive(false);
            audioSourcePool.Enqueue(audioSource);
        }
    }

    private IAudioService CreateAudioService(string name, bool loop = false)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(audioSettings.audioHolder);
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.loop = loop;
        return new AudioSourceWrapper(audioSource);
    }

    private AudioSource GetPooledAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            // Expand the pool if needed
            GameObject obj = new GameObject($"PooledAudioSource_Extra");
            obj.transform.SetParent(audioSettings.audioHolder);
            return obj.AddComponent<AudioSource>();
        }
    }

    private void ReturnToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    public void PlayAudioClip(string audioClipName)
    {
        AudioClip clip = System.Array.Find(audioSettings.sfxClips, x => x.name == audioClipName);
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: Sound effect '{audioClipName}' not found!");
            return;
        }

        AudioSource sfxPlayer = GetPooledAudioSource();
        sfxPlayer.clip = clip;
        sfxPlayer.volume = audioSettings.sfxVolume;
        sfxPlayer.Play();

        FeedbackManager.Instance.StartCoroutine(ReturnToPoolAfterPlayback(sfxPlayer));
    }

    public void PlayAudioClipAtOneShot(string clipName)
    {
        if (!sfxPlayers.ContainsKey(clipName))
        {
            Debug.LogWarning($"AudioManager: Sound effect '{clipName}' not found!");
            return;
        }

        AudioSource source = GetPooledAudioSource();
        source.volume = audioSettings.sfxVolume;

        // Use the `PlayOneShot` method we just added
        (sfxPlayers[clipName] as AudioSourceWrapper)?.PlayOneShot(source.clip);

        FeedbackManager.Instance.StartCoroutine(ReturnToPoolAfterPlayback(source));
    }

    private IEnumerator ReturnToPoolAfterPlayback(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        ReturnToPool(source);
    }

    public void PlayMusic(AudioClip clip)
    {
        if ((musicPlayer as AudioSourceWrapper)?.CurrentClip == clip) return;
        FeedbackManager.Instance.StartCoroutine(CrossfadeMusic(clip, audioSettings.crossfadeDuration));
    }

    public void PlayUISound(AudioClip clip)
    {
        AudioSource source = GetPooledAudioSource();
        source.volume = audioSettings.uiSoundVolume;
        source.PlayOneShot(clip);
        FeedbackManager.Instance.StartCoroutine(ReturnToPoolAfterPlayback(source));
    }

    public void StopAll()
    {
        musicPlayer.Stop();
        foreach (var player in sfxPlayers.Values)
            player.Stop();

        uiSoundPlayer.Stop();
    }

    public void PauseAll()
    {
        musicPlayer.Pause();
        foreach (var player in sfxPlayers.Values)
            player.Pause();

        uiSoundPlayer.Pause();
    }

    public void ResumeAll()
    {
        musicPlayer.Resume();
        foreach (var player in sfxPlayers.Values)
            player.Resume();

        uiSoundPlayer.Resume();
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
    {
        IAudioService newMusicPlayer = CreateAudioService("TempMusicPlayer", true);
        newMusicPlayer.Play(newClip);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicPlayer.SetVolume(Mathf.Lerp(audioSettings.musicVolume, 0f, time / duration));
            newMusicPlayer.SetVolume(Mathf.Lerp(0f, audioSettings.musicVolume, time / duration));
            yield return null;
        }

        musicPlayer.Stop();
        musicPlayer = newMusicPlayer;
    }

    private void ApplyVolumeSettings()
    {
        AudioListener.volume = audioSettings.masterVolume;
        musicPlayer.SetVolume(audioSettings.musicVolume);
        uiSoundPlayer.SetVolume(audioSettings.uiSoundVolume);

        foreach (var player in sfxPlayers.Values)
            player.SetVolume(audioSettings.sfxVolume);
    }
}