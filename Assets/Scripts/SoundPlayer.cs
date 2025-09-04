using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [Header("Volume")]
    [Range(0f, 1f)] public float volume;
    
    [Header("Sound Clips")]
    public List<SoundClips> soundClips;
    
    [Header("Pitch Settings")]
    public bool randomPitch;
    public Vector2 pitchRange;
    
    [Header("Sound Distance Settings")]
    public bool useSoundDistance;
    public Vector2 soundDistance;

    private Dictionary<string, AudioClip[]> _soundLibrary;
    private AudioSource _audioSource;
    private int _lastIndex = -1;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        _soundLibrary = new Dictionary<string, AudioClip[]>();
        foreach (var soundClip in soundClips)
        {
            if (!_soundLibrary.ContainsKey(soundClip.key))
            {
                _soundLibrary.Add(soundClip.key, soundClip.audioClips);
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        
        if (randomPitch)
        {
            _audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        }
        
        if (useSoundDistance)
        {
            _audioSource.spatialBlend = 1;
            _audioSource.minDistance = soundDistance.x;
            _audioSource.maxDistance = soundDistance.y;
            _audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        _audioSource.volume = volume;
        _audioSource.Play();
    }
    
    [Button]
    public void PlayRandom(string key)
    {
        if (_soundLibrary.TryGetValue(key, out var clips))
        {
            PlayRandomSound(clips);
        }
        else
        {
            Debug.LogWarning($"No sounds found for key: {key}");
        }
    }
    
    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        
        // shuffle
        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (index == _lastIndex && clips.Length > 1);
        
        _lastIndex = index;
        _audioSource.clip = clips[index];
        
        if (randomPitch)
        {
            _audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        }

        if (useSoundDistance)
        {
            _audioSource.spatialBlend = 1;
            _audioSource.minDistance = soundDistance.x;
            _audioSource.maxDistance = soundDistance.y;
            _audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        _audioSource.volume = volume;
        _audioSource.Play();
    }
}

[Serializable]
public class SoundClips
{
    public string key;
    public AudioClip[] audioClips;
}
