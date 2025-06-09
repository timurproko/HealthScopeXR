using Atomic.Entities;
using Game;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;
using Tween = PrimeTween.Tween;

public class FilterBehaviour : IInit, IUpdate, IDispose
{
    private readonly RawImage _image;
    private readonly Material[] _filterMaterials;
    private readonly Material _materialAI;
    private readonly AudioClip _magicSound;

    private InteractableUnityEventWrapper _filterButton;
    private int _currentIndex = -1;
    private IEntity _entity;
    private AudioSource _audioSource;

    public FilterBehaviour(RawImage image, Material[] filterMaterials, Material materialAI, AudioClip magicSound)
    {
        _image = image;
        _filterMaterials = filterMaterials;
        _materialAI = materialAI;
        _magicSound = magicSound;
    }

    public void Init(in IEntity entity)
    {
        _entity = entity;
        _filterButton = entity.GetButtonFilter();
        _filterButton.WhenSelect.AddListener(ToggleFilters);
        _entity.GetFinishedEvent().Subscribe(FadeOutMagicSound);
        _audioSource = _entity.GetAudioSource();
    }

    private void FadeOutMagicSound()
    {
        Tween.Custom(_audioSource.volume, 0f, 1f, val => _audioSource.volume = val)
            .OnComplete(() => _audioSource.Stop());
    }

    public void OnUpdate(in IEntity entity, in float deltaTime)
    {
        SetAIMaterial();
    }

    public void Dispose(in IEntity entity)
    {
        _filterButton.WhenSelect.RemoveListener(ToggleFilters);
        _entity.GetFinishedEvent().Unsubscribe(FadeOutMagicSound);
    }

    private void ToggleFilters()
    {
        _currentIndex++;

        if (_currentIndex >= _filterMaterials.Length)
        {
            _image.material = null;
            _currentIndex = -1;
        }
        else
        {
            _image.material = _filterMaterials[_currentIndex];
        }
    }

    private void SetAIMaterial()
    {
        if (_entity.GetIsAIWorking())
        {
            _image.material = _materialAI;
            _audioSource.volume = 0.25f;
            _audioSource.PlayOneShot(_magicSound);
        }
        else
        {
            _image.material = null;
        }
    }
}