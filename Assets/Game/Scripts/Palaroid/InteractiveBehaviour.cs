using Atomic.Elements;
using Atomic.Entities;
using Shapes;
using UnityEngine;

public class InteractiveBehaviour : IInit
{
    private readonly TriggerEventReceiver _triggerEventReceiver;
    private readonly Rectangle _frame;

    public InteractiveBehaviour(TriggerEventReceiver triggerEventReceiver, Rectangle frame)
    {
        _triggerEventReceiver = triggerEventReceiver;
        _frame = frame;
    }

    public void Init(in IEntity entity)
    {
        _triggerEventReceiver.OnEntered += OnTriggerEntered;
        _triggerEventReceiver.OnExited += OnTriggerExited;
        _frame.enabled = false;
    }

    private void OnTriggerEntered(Collider obj)
    {
        _frame.enabled = true;
    }
    
    private void OnTriggerExited(Collider obj)
    {
        _frame.enabled = false;
    }
}