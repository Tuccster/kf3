using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    public float health;

    public void HealthDelta(float delta)
    {
        health += delta;
        if (health <= 0.0f) Destroy(gameObject);
    }
}
