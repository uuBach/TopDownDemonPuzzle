using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    private AudioSource musicSource;       // Для фоновой музыки
    private AudioSource loopedAudioSource; // Для звуков шагов
    private Dictionary<AudioClip, AudioSource> audioSources = new Dictionary<AudioClip, AudioSource>();

    private void Awake()
    {
        instance = this;

        // Инициализация AudioSource для фоновой музыки и установки на повтор
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // Инициализация AudioSource для звуков шагов и установки на повтор
        loopedAudioSource = gameObject.AddComponent<AudioSource>();
        loopedAudioSource.loop = true;
    }

    // Метод для воспроизведения звукового эффекта
    public void PlaySound(AudioClip clip)
    {
        if (audioSources.ContainsKey(clip))
        {
            audioSources[clip].Play();
        }
        else
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = clip;
            newSource.Play();
            audioSources[clip] = newSource;
        }
    }

    public void StopSound(AudioClip clip)
    {
        if (audioSources.ContainsKey(clip))
        {
            AudioSource source = audioSources[clip];
            source.Stop();
            Destroy(source);
            audioSources.Remove(clip);
        }
    }

    // Метод для воспроизведения фоновой музыки
    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (musicSource.clip != musicClip)
        {
            musicSource.clip = musicClip;
        }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    // Метод для воспроизведения звуков шагов (или любого звука на повторе)
    public void PlayLoopedAudio(AudioClip loopClip)
    {
        if (loopedAudioSource.clip != loopClip)
        {
            loopedAudioSource.clip = loopClip;
        }

        if (!loopedAudioSource.isPlaying)
        {
            loopedAudioSource.Play();
        }
    }

    // Метод для остановки конкретного звука, переданного в параметрах
    public void StopLoopedAudio(AudioClip loopClip)
    {
        if (loopedAudioSource.clip == loopClip && loopedAudioSource.isPlaying)
        {
            loopedAudioSource.Stop();
        }
    }
    public void StopAllSounds()
    {
        // Остановить фоновую музыку
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // Остановить зацикленный звук (например, шаги)
        if (loopedAudioSource.isPlaying)
        {
            loopedAudioSource.Stop();
        }

        // Остановить все звуковые эффекты, хранящиеся в audioSources
        foreach (var source in audioSources.Values)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}



