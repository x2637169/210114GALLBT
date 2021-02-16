using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern_HSMediator;
public abstract class IGameSystem:MonoBehaviour
{
    protected AbstractSlotMediator m_SlotMediatorController = null;
    public void setMediator(AbstractSlotMediator mediator)
    {
        this.m_SlotMediatorController = mediator;
    }
    public virtual void Initialize() { }
    public virtual void Release() { }
  
}
