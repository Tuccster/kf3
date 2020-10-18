using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyLimb : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float damageMultiplier = 1f;

    private Dummy parent;

    private void Awake()
    {
        parent = transform.parent.GetComponent<Dummy>();
    }

    public void HealthDelta(float delta)
    {
        parent.TakeDamage(delta * damageMultiplier, transform.name);
    }
}
