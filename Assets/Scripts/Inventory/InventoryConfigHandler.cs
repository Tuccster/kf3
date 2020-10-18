using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryConfigHandler : MonoBehaviour
{
    [Header("Resources")]
    public InventoryUI invPlayer;
    public InventoryUI invSmallContainer;
    public InventoryUI invLargeContainer;

    private Dictionary<InventoryType, InventoryUI> inventoryDict;

    public enum InventoryType
    {
        Player,
        SmallContainer,
        LargeContainer
    }

    private void Awake()
    {
        // Construct inventory dictionary
        inventoryDict = new Dictionary<InventoryType, InventoryUI>()
        {
            {InventoryType.Player, invPlayer},
            {InventoryType.SmallContainer, invSmallContainer},
            {InventoryType.LargeContainer, invLargeContainer}
        };
    }

    /// <summary> Only call in Start()!</summary>
    public InventoryUI GetInventoryType(InventoryType _inventoryType)
    {
        return inventoryDict[_inventoryType];
    }

    public void OpenConfig(Inventory _inventory)
    {
        // Always open player inventory
        invPlayer.gameObject.SetActive(true);
        if (_inventory.inventoryType == InventoryType.Player) return;

        InventoryUI inv = inventoryDict[_inventory.inventoryType];
        inv.UpdateAllSlots(_inventory.itemStacks);
        inventoryDict[_inventory.inventoryType].gameObject.SetActive(true);
    }

    
}
