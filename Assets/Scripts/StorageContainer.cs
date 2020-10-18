using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class StorageContainer : MonoBehaviour, IInteractable
{
    public void Interact(Transform invoker)
    {
        Inventory inv = GetComponent<Inventory>();
        inv.inventoryConfigHandler.OpenConfig(inv);
    }
}
