using System.Collections;
using Atomic.Entities;
using Oculus.Interaction;
using UnityEngine;

public class AIBehaviour : IInit
{
    private readonly MicRecorder _recorder;
    private readonly GroqTranscriber _transcriber;
    private readonly GroqVision _vision;
    private readonly OpenAITTS _speech;
    private readonly InteractableUnityEventWrapper _interaction;
    private readonly MonoBehaviour _host;

    public AIBehaviour(MicRecorder recorder, GroqTranscriber transcriber, GroqVision vision, OpenAITTS speech,
        InteractableUnityEventWrapper interaction)
    {
        _recorder = recorder;
        _transcriber = transcriber;
        _vision = vision;
        _speech = speech;
        _interaction = interaction;
        _host = _recorder;
    }

    public void Init(in IEntity entity)
    {
        _interaction.WhenSelect.AddListener(OnSelected);
        _interaction.WhenUnselect.AddListener(OnUnselected);
    }

    private void OnSelected()
    {
        _recorder.StartRecording();
    }

    private void OnUnselected()
    {
        _recorder.StopRecording();
        _host.StartCoroutine(Pipeline());
    }

    private IEnumerator Pipeline()
    {
        yield return new WaitUntil(() => _recorder.IsReady());
        _transcriber.TranscribeAudio();

        yield return new WaitUntil(() => _transcriber.IsReady());
        _vision.RequestVision();

        yield return new WaitUntil(() => _vision.IsReady());
        _speech.GenerateSpeech();
    }
}