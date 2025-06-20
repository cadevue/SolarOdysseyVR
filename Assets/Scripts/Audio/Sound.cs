using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    public string Name;
    [Range(0f, 1f)]
    public float Volume = 0.6f;
    [Range(0.1f, 3f)]
    public float Pitch = 1f;
    public bool Loop = false;

    [HideInInspector]
    public AudioSource Source;
}