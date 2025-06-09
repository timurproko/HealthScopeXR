using Atomic.Entities;
using Game;
using Oculus.Interaction;
using UnityEngine;

public class UIBehaviour : IInit, IDispose

{
    private readonly GameObject _normalText;
    private readonly GameObject _rainbowText;
    
    private IEntity _entity;

    public UIBehaviour(GameObject normalText, GameObject rainbowText)
    {
        _normalText = normalText;
        _rainbowText = rainbowText;
    }

    public void Init(in IEntity entity)
    {
        _entity = entity;
        entity.GetStartedEvent().Subscribe(EnableRainbow);
        entity.GetFinishedEvent().Subscribe(DisableRainbow);
        DisableRainbow();
    }

    public void Dispose(in IEntity entity)
    {
        entity.GetStartedEvent().Unsubscribe(EnableRainbow);
        entity.GetFinishedEvent().Unsubscribe(DisableRainbow);
    }

    private void EnableRainbow()
    {
        _normalText.SetActive(false);
        _rainbowText.SetActive(true);
        _entity.GetButtonAI().GetComponentInChildren<InteractableUnityEventWrapper>().enabled = true;
    }

    private void DisableRainbow()
    {
        _normalText.SetActive(true);
        _rainbowText.SetActive(false);
        _entity.GetButtonAI().GetComponentInChildren<InteractableUnityEventWrapper>().enabled = false;
    }
}