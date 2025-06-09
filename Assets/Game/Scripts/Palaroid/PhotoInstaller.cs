using Atomic.Elements;
using Atomic.Entities;
using Game;
using Oculus.Interaction;
using PassthroughCameraSamples;
using Shapes;
using UnityEngine;
using UnityEngine.UI;

public class PhotoInstaller : SceneEntityInstaller
{
    [Header("Camera Settings")]
    [SerializeField] private WebCamTextureManager _webCamHeadset;
    [SerializeField] private WebCamTextureManagerPC _webCamPC;
    [SerializeField] private bool _enableDebugCamera;
    [SerializeField] private RawImage _image;
    [SerializeField] private Material[] _filterMaterials;
    [SerializeField] private Material _materialAI;

    [Header("UI Settings")]
    [SerializeField] private Text _text;
    [SerializeField] private Text _description;
    [SerializeField] private Disc _counter;
    [SerializeField] private InteractableUnityEventWrapper _buttonCapture;
    [SerializeField] private InteractableUnityEventWrapper _buttonFilter;
    [SerializeField] private InteractableUnityEventWrapper _buttonAI;
    [SerializeField] private TriggerEventReceiver _triggerEventReceiver;
    [SerializeField] private Rectangle _frame;
    [SerializeField] private GameObject _textNormal;
    [SerializeField] private GameObject _textRainbow;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _cameraSound;
    [SerializeField] private AudioClip _countdownSound;
    [SerializeField] private AudioClip _magicSound;

    public override void Install(IEntity entity)
    {
        entity.AddIsDebug(_enableDebugCamera);
        entity.AddIsCaptured(false);
        entity.AddIsAIWorking(false);
        entity.AddCounter(_counter);
        entity.AddText(_text);
        entity.AddDescription(_description);
        entity.AddAudioSource(_audioSource);
        entity.AddButtonCapture(_buttonCapture);
        entity.AddButtonFilter(_buttonFilter);
        entity.AddButtonAI(_buttonAI);
        entity.AddSnapshot(Texture2D.whiteTexture);
        entity.AddTriggerReceiver(_triggerEventReceiver);
        entity.AddFinishedEvent(new BaseEvent());
        entity.AddStartedEvent(new BaseEvent());
        
        entity.AddBehaviour(new CaptureBehaviour(_webCamHeadset, _webCamPC, _image, _countdownSound));
        entity.AddBehaviour(new FilterBehaviour(_image, _filterMaterials, _materialAI, _magicSound));
        entity.AddBehaviour(new AIBehaviour());
        entity.AddBehaviour(new InteractiveBehaviour(_triggerEventReceiver, _frame));
        entity.AddBehaviour(new UIBehaviour(_textNormal, _textRainbow));
    }
}