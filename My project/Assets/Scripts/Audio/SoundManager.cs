using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    private AudioSource musicSource;       // ��� ������� ������
    private AudioSource loopedAudioSource; // ��� ������ �����
    private Dictionary<AudioClip, AudioSource> audioSources = new Dictionary<AudioClip, AudioSource>();

    private void Awake()
    {
        instance = this;

        // ������������� AudioSource ��� ������� ������ � ��������� �� ������
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // ������������� AudioSource ��� ������ ����� � ��������� �� ������
        loopedAudioSource = gameObject.AddComponent<AudioSource>();
        loopedAudioSource.loop = true;
    }

    // ����� ��� ��������������� ��������� �������
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

    // ����� ��� ��������������� ������� ������
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

    // ����� ��� ��������������� ������ ����� (��� ������ ����� �� �������)
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

    // ����� ��� ��������� ����������� �����, ����������� � ����������
    public void StopLoopedAudio(AudioClip loopClip)
    {
        if (loopedAudioSource.clip == loopClip && loopedAudioSource.isPlaying)
        {
            loopedAudioSource.Stop();
        }
    }
    public void StopAllSounds()
    {
        // ���������� ������� ������
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // ���������� ����������� ���� (��������, ����)
        if (loopedAudioSource.isPlaying)
        {
            loopedAudioSource.Stop();
        }

        // ���������� ��� �������� �������, ���������� � audioSources
        foreach (var source in audioSources.Values)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}



