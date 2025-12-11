using UnityEngine;

namespace Managers
{
public class AudioManager: MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources; // Overlapping SFX
    private int _nextSourceIndex;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f) {
        if (Instance == null || clip == null) return;

        AudioSource src = Instance.sfxSources[Instance._nextSourceIndex];
        Instance._nextSourceIndex = (Instance._nextSourceIndex + 1) % Instance.sfxSources.Length;

        src.PlayOneShot(clip, volume);
    }
}
}