using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    //void UsePrimary();
    //void UseSecondary();
}

public interface IDamageable
{
    void HealthDelta(float delta);
}

public interface IInteractable
{
    void Interact(Transform invoker);
}