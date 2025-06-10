using Atomic.Entities;
using Game;
using UnityEngine;

public class SoundBehaviour : IInit
{
    private AudioSource _audioSource;
    private readonly AudioClip _audioClip;

    public SoundBehaviour(AudioClip audioClip)
    {
        _audioClip = audioClip;
    }

    public void Init(in IEntity entity)
    {
        _audioSource = entity.GetAudioSource();
        _audioSource.PlayOneShot(_audioClip);
    }
}