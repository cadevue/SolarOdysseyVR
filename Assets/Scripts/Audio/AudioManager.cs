using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public Sound[] SoundList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        foreach (Sound sound in SoundList)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    public void Play(string name)
    {
        Sound sound = System.Array.Find(SoundList, s => s.Name == name);
        if (sound != null)
        {
            sound.Source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }

    public void PlayOneShot(string name)
    {
        Sound sound = System.Array.Find(SoundList, s => s.Name == name);
        if (sound != null)
        {
            sound.Source.PlayOneShot(sound.Clip, sound.Volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }

    public void Stop(string name)
    {
        Sound sound = System.Array.Find(SoundList, s => s.Name == name);
        if (sound != null)
        {
            sound.Source.Stop();
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }
}
