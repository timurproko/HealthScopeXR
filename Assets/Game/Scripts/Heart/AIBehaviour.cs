using System.Collections;
using Atomic.Entities;
using Febucci.UI;
using Oculus.Interaction;
using UnityEngine;

public class AIBehaviour : IInit
{
    private readonly MicRecorder _recorder;
    private readonly GroqTranscriber _transcriber;
    private readonly GroqVision _vision;
    private readonly OpenAIResponder _responder;
    private readonly OpenAITTS _speech;
    private readonly InteractableUnityEventWrapper _interaction;
    private readonly TextAnimator_TMP _textAnimator;
    private readonly MonoBehaviour _host;

    public AIBehaviour(MicRecorder recorder, GroqTranscriber transcriber, GroqVision vision, OpenAIResponder responder, OpenAITTS speech,
        InteractableUnityEventWrapper interaction, TextAnimator_TMP textAnimator)
    {
        _recorder = recorder;
        _transcriber = transcriber;
        _vision = vision;
        _responder = responder;
        _speech = speech;
        _interaction = interaction;
        _textAnimator = textAnimator;
        _host = _recorder;
    }

    public void Init(in IEntity entity)
    {
        _interaction.WhenSelect.AddListener(OnSelected);
        _interaction.WhenUnselect.AddListener(OnUnselected);
        _textAnimator.SetText("Ask AI");
    }

    private void OnSelected()
    {
        _speech.StopSpeech();
        _recorder.StartRecording();
    }

    private void OnUnselected()
    {
        _recorder.StopRecording();
        _textAnimator.SetText("<rainb>Ask AI");
        _host.StartCoroutine(Pipeline());
    }

    private IEnumerator Pipeline()
    {
        yield return new WaitUntil(() => _recorder.IsReady());
        _transcriber.TranscribeAudio();

        yield return new WaitUntil(() => _transcriber.IsReady());
        _vision.RequestVision();

        yield return new WaitUntil(() => _vision.IsReady());
        _responder.AskChatGPT();
        
        yield return new WaitUntil(() => _responder.IsReady());
        _speech.GenerateSpeech();
        
        yield return new WaitForSeconds(5f);
        _textAnimator.SetText("Ask AI");
    }
}