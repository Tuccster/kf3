using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropped : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [Range(1, 1000)]
    public int amount = 1;
    
    [Header("Debug")]
    public bool destoryOnPickup = true;

    [Header("Resources")]
    public ItemBase item;

    public void Interact(Transform invoker)
    {
        if (invoker.GetComponent<Inventory>() != null && item != null)
            invoker.GetComponent<Inventory>().AddItem(item, amount);

        if (destoryOnPickup)
            Destroy(gameObject);
    }
}
