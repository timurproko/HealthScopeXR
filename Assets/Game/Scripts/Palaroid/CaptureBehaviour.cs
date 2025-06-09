using System;
using Atomic.Entities;
using Game;
using Oculus.Interaction;
using PassthroughCameraSamples;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Tween = PrimeTween.Tween;

public class CaptureBehaviour : IInit, IUpdate, IDispose
{
    private readonly WebCamTextureManager _webCamHeadset;
    private readonly WebCamTextureManagerPC _webCamPC;
    private readonly RawImage _image;
    private readonly AudioClip _countdownSound;

    private IEntity _entity;

    private InteractableUnityEventWrapper _captureButton;
    private Text _text;
    private Disc _counter;
    private WebCamTexture _camTex;
    private Texture2D _snapshotTex;
    private AudioSource _audioSource;

    public CaptureBehaviour(WebCamTextureManager webCamHeadset, WebCamTextureManagerPC webCamPC, RawImage image, AudioClip countdownSound)
    {
        _webCamHeadset = webCamHeadset;
        _webCamPC = webCamPC;
        _image = image;
        _countdownSound = countdownSound;
    }

    public void Init(in IEntity entity)
    {
        _entity = entity;
        _captureButton = entity.GetButtonCapture();
        _text = entity.GetText();
        _counter = entity.GetCounter();
        _audioSource = entity.GetAudioSource();

        _captureButton.WhenSelect.AddListener(ToggleCaptureOrDelete);
        _camTex = entity.GetIsDebug() ? _webCamPC.WebCamTexture : _webCamHeadset.WebCamTexture;
        _image.texture = _camTex;
        _text.text = "Camera 1";
    }

    public void Dispose(in IEntity entity)
    {
        _captureButton.WhenSelect.RemoveListener(ToggleCaptureOrDelete);
        _camTex.Stop();
    }

    public void OnUpdate(in IEntity entity, in float deltaTime)
    {
        if (_entity.GetIsCaptured())
        {
            SetButtonLabel("Delete");
        }
        else
        {
            SetButtonLabel("Capture");
        }
    }

    private void ToggleCaptureOrDelete()
    {
        if (_entity.GetIsCaptured())
        {
            DeleteSnapshot();
            _entity.GetFinishedEvent().Invoke();
        }
        else
        {
            CaptureSnapshot();
        }
    }

    private void CaptureSnapshot()
    {
        if (!_camTex || !_camTex.isPlaying || !_camTex.didUpdateThisFrame)
            return;

        _text.text = "";
        _audioSource.volume = 0.25f;
        _audioSource.PlayOneShot(_countdownSound);
        const float duration = 3f;

        Tween.Custom(0f, 6.28319f, duration, val => _counter.AngRadiansEnd = val)
            .OnComplete(() =>
            {
                _counter.AngRadiansEnd = 0;

                _snapshotTex = new Texture2D(_camTex.width, _camTex.height, TextureFormat.RGB24, false);
                _snapshotTex.SetPixels(_camTex.GetPixels());
                _snapshotTex.Apply();

                _text.text = DateTime.Now.ToString("dd/MM/yyyy h.mmtt").ToLower();

                _image.texture = _snapshotTex;

                _entity.SetSnapshot(_snapshotTex);
                _entity.SetIsCaptured(true);
                _entity.GetStartedEvent().Invoke();
            });
    }

    private void DeleteSnapshot()
    {
        _text.text = "Camera 1";
        _image.texture = _camTex;

        _entity.SetIsCaptured(false);
    }

    private void SetButtonLabel(string label)
    {
        var tmp = _captureButton.GetComponentInChildren<TMP_Text>();
        if (tmp) tmp.text = label;
    }
}