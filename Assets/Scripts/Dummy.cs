using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Settings")]
    public float health;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage, string bodyPart)
    {
        Debug.Log($"{bodyPart} took {+damage}");
        health += damage;
        if (health <= 0)
            Destroy(gameObject);
    }
}
