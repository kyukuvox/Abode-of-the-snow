using UnityEngine;

public class SoundSettings : MonoBehaviour
{
    public static SoundSettings Instance;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    public static float MusicVolume { get; private set; } = 1f;
    public static float SFXVolume { get; private set; } = 1f;

    void Awake()
    {
        Instance = this;
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    public static void PlaySound(AudioClip clip, float volume, MonoBehaviour caller)
    {
        if (clip == null || caller == null) return;
        caller.StartCoroutine(PlaySoundRoutine(clip, volume * SFXVolume));
    }

    private static System.Collections.IEnumerator PlaySoundRoutine(AudioClip clip, float volume)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.spatialBlend = 0f;
        tempSource.Play();
        Object.Destroy(tempAudio, clip.length);
        yield return null;
    }

    public static void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        if (MusicManager.Instance != null)
            MusicManager.Instance.ApplyMusicVolume(volume);
        if (CardGameManager.Instance != null)
            CardGameManager.Instance.ApplyMusicVolume(volume);
    }

    public static void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);
        PlayerPrefs.Save();
    }
}