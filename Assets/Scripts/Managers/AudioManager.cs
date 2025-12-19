using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
public class AudioManager: MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _musicSource;
    private List<AudioSource> _sfxSources = new(); // Overlapping SFX
    private int _nextSourceIndex;

    [SerializeField] private AudioClip[] levelSoundtracks;
    [SerializeField] private AudioClip menuSoundtrack;
    [SerializeField] private AudioClip bossSoundtrack;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void PlayMenuMusic(float volume = 1f)
    {
        PlayMusic(menuSoundtrack, volume);
    }

    public void PlayLevelMusic(int trackIndex, float volume = 1f)
    {
        if (trackIndex < 0 || trackIndex >= levelSoundtracks.Length) return;
        PlayMusic(levelSoundtracks[trackIndex], volume);
    }

    public void PlayBossMusic(float volume = 1f)
    {
        PlayMusic(bossSoundtrack, volume);
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void ChangeMusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource src = GetAvailableSFXSource();
        src.PlayOneShot(clip, volume);
    }

    private AudioSource GetAvailableSFXSource()
    {
        // Try to reuse an idle source
        foreach (var source in _sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // None free -> create a new one
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        _sfxSources.Add(newSource);
        return newSource;
    }
}
}