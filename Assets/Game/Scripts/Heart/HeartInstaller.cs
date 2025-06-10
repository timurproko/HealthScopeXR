using Atomic.Entities;
using Game;
using UnityEngine;

public class HeartInstaller : SceneEntityInstaller
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _heartSound;

    public override void Install(IEntity entity)
    {
        entity.AddAudioSource(_audioSource);
        
        entity.AddBehaviour(new SoundBehaviour(_heartSound));
    }
}