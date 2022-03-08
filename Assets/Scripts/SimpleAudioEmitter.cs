using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This struct just helps with tying a name to an audio clip so we can call it by name
[System.Serializable]
public struct SimpleAudio
{
    public string name;
    public AudioClip clip;
}

/// <summary>
/// Quick and simple script for playing audio
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SimpleAudioEmitter : MonoBehaviour
{
    public List<SimpleAudio> clips = new List<SimpleAudio>();
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayNoise(string clipName, float volume = 1f)
    {
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                source.PlayOneShot(clip.clip, volume);
            }
        }
    }
}
