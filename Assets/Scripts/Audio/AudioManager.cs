using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public Sound[] SoundList;
    public RandomSound[] RandomSoundList;

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

        foreach (RandomSound randomSound in RandomSoundList)
        {
            randomSound.Source = gameObject.AddComponent<AudioSource>();
            randomSound.Source.volume = randomSound.Volume;
            randomSound.Source.pitch = randomSound.Pitch;
            randomSound.Source.loop = randomSound.Loop;
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

    public void PlayRandom(string name)
    {
        RandomSound randomSound = System.Array.Find(RandomSoundList, s => s.Name == name);
        if (randomSound != null && randomSound.Clips.Length > 0)
        {
            int index = Random.Range(0, randomSound.Clips.Length);
            randomSound.Source.clip = randomSound.Clips[index];
            randomSound.Source.Play();
        }
        else
        {
            Debug.LogWarning($"Random sound '{name}' not found or has no clips!");
        }
    }

    public void PlayRandomOneShot(string name)
    {
        RandomSound randomSound = System.Array.Find(RandomSoundList, s => s.Name == name);
        if (randomSound != null && randomSound.Clips.Length > 0)
        {
            int index = Random.Range(0, randomSound.Clips.Length);
            randomSound.Source.PlayOneShot(randomSound.Clips[index], randomSound.Volume);
        }
        else
        {
            Debug.LogWarning($"Random sound '{name}' not found or has no clips!");
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

    public void StopRandom(string name)
    {
        RandomSound randomSound = System.Array.Find(RandomSoundList, s => s.Name == name);
        if (randomSound != null)
        {
            randomSound.Source.Stop();
        }
        else
        {
            Debug.LogWarning($"Random sound '{name}' not found!");
        }
    }

    public void StopAll()
    {
        foreach (Sound sound in SoundList)
        {
            sound.Source.Stop();
        }

        foreach (RandomSound randomSound in RandomSoundList)
        {
            randomSound.Source.Stop();
        }
    }
}
