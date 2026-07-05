using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages all audio including 3D spatial audio, ambience, and dynamic music.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private int maxSFXSources = 16;
    
    private Queue<AudioSource> sfxSourcePool = new Queue<AudioSource>();
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicTracks = new Dictionary<string, AudioClip>();
    
    private bool isInitialized = false;
    private float currentMusicIntensity = 0f;
    
    public event Action OnAmbienceStarted;
    public event Action OnMusicStarted;
    
    public void Initialize()
    {
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (ambienceSource == null) ambienceSource = gameObject.AddComponent<AudioSource>();
        
        musicSource.loop = true;
        musicSource.priority = 50;
        musicSource.volume = 0.7f;
        musicSource.spatialBlend = 0f;
        
        ambienceSource.loop = true;
        ambienceSource.priority = 60;
        ambienceSource.volume = 0.4f;
        ambienceSource.spatialBlend = 0f;
        
        for (int i = 0; i < maxSFXSources; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.priority = 128;
            sfxSource.spatialBlend = 1f;
            sfxSourcePool.Enqueue(sfxSource);
        }
        
        LoadSoundLibrary();
        isInitialized = true;
        
        Debug.Log("[AudioManager] Initialized with " + maxSFXSources + " SFX channels");
    }
    
    private void LoadSoundLibrary()
    {
        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Audio/SFX");
        foreach (AudioClip clip in sfxClips)
            soundLibrary[clip.name] = clip;
        
        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("Audio/Music");
        foreach (AudioClip clip in musicClips)
            musicTracks[clip.name] = clip;
        
        Debug.Log($"[AudioManager] Loaded {soundLibrary.Count} SFX and {musicTracks.Count} music tracks");
    }
    
    public void PlaySFX(string soundName, Vector3 position, float volume = 1f)
    {
        if (!soundLibrary.ContainsKey(soundName)) return;
        
        AudioSource source = GetAvailableSFXSource();
        if (source == null) return;
        
        source.clip = soundLibrary[soundName];
        source.volume = volume;
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.Play();
    }
    
    public void PlayAmbience()
    {
        if (!isInitialized) return;
        ambienceSource.volume = 0.3f;
        ambienceSource.Play();
        OnAmbienceStarted?.Invoke();
        Debug.Log("[AudioManager] Ambience started");
    }
    
    public void StopAmbience()
    {
        if (ambienceSource != null) ambienceSource.Stop();
    }
    
    public void StartDynamicMusic(int hour)
    {
        if (!isInitialized) return;
        string trackName = GetMusicTrackForHour(hour);
        if (musicTracks.ContainsKey(trackName))
        {
            musicSource.clip = musicTracks[trackName];
            musicSource.volume = 0.5f;
            musicSource.Play();
            OnMusicStarted?.Invoke();
            Debug.Log($"[AudioManager] Playing: {trackName}");
        }
    }
    
    private string GetMusicTrackForHour(int hour)
    {
        if (hour < 1) return "ambient_midnight";
        if (hour < 3) return "ambient_early_night";
        if (hour < 5) return "ambient_deep_night";
        return "ambient_late_night";
    }
    
    public void UpdateMusicIntensity(float sanityPercentage)
    {
        currentMusicIntensity = 1f - (sanityPercentage / 100f);
        musicSource.volume = Mathf.Lerp(0.3f, 0.8f, currentMusicIntensity);
    }
    
    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxSourcePool)
        {
            if (!source.isPlaying) return source;
        }
        
        AudioSource oldestSource = sfxSourcePool.Dequeue();
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.priority = 128;
        newSource.spatialBlend = 1f;
        sfxSourcePool.Enqueue(newSource);
        return newSource;
    }
    
    public void PauseAll() { AudioListener.pause = true; }
    public void ResumeAll() { AudioListener.pause = false; }
    
    public void StopAll()
    {
        musicSource.Stop();
        ambienceSource.Stop();
        foreach (AudioSource source in sfxSourcePool) source.Stop();
    }
    
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
    
    public void PlayWhisper(Vector3 position) => PlaySFX("whisper_" + Random.Range(1, 4), position, 0.6f);
    public void PlayCreak(Vector3 position) => PlaySFX("creak_" + Random.Range(1, 5), position, 0.4f);
    public void PlayChildrenLaughing(Vector3 position) => PlaySFX("children_laughing", position, 0.8f);
    public void PlaySarcophagusRattle(Vector3 position) => PlaySFX("sarcophagus_rattle", position, 1f);
    public void PlayFootstep(Vector3 position, string surfaceType = "stone") => PlaySFX($"footstep_{surfaceType}_" + Random.Range(1, 4), position, 0.3f);
}
