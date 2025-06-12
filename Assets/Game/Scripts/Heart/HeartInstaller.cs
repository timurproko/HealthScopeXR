using Atomic.Entities;
using Febucci.UI;
using Game;
using Oculus.Interaction;
using UnityEngine;

public class HeartInstaller : SceneEntityInstaller
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _heartSound;
    [SerializeField] private MicRecorder _recorder;
    [SerializeField] private GroqTranscriber _transcriber;
    [SerializeField] private GroqVision _vision;
    [SerializeField] private OpenAITTS _speech;
    [SerializeField] private OpenAIResponder _responder;
    [SerializeField] private InteractableUnityEventWrapper _interaction;
    [SerializeField] private TextAnimator_TMP textAnimator;

    
    public override void Install(IEntity entity)
    {
        entity.AddAudioSource(_audioSource);
        
        entity.AddBehaviour(new SoundBehaviour(_heartSound));
        entity.AddBehaviour(new AIBehaviour(_recorder, _transcriber, _vision, _responder, _speech, _interaction, textAnimator));
    }
}