using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Resources")]
    public InventorySlot[] slots;
    public Image moveItemIcon;

    private void Awake()
    {
        // Assign each slot its slot index
        for (int i = 0; i < slots.Length; i++)
            slots[i].slotIndex = i;

        UpdateMoveItemIcon(null);
    }

    private void Update()
    {
        moveItemIcon.rectTransform.position = Input.mousePosition;
    }

    public void UpdateSlot(Inventory.ItemStack[] itemStacks, int slotIndex)
    {
        Image slotIcon = slots[slotIndex].icon;
        Text slotAmount = slots[slotIndex].amount;

        if (itemStacks[slotIndex].item == null)
        {
            slotIcon.sprite = null;
            slotIcon.color = Color.clear;
            slotAmount.text = string.Empty;
        }
        else
        {
            slotIcon.sprite = itemStacks[slotIndex].item.icon;
            slotIcon.color = Color.white;
            if (itemStacks[slotIndex].amount > 1)
                slotAmount.text = itemStacks[slotIndex].amount.ToString();
            else 
                slotAmount.text = string.Empty;
        }
    }

    public void UpdateAllSlots(Inventory.ItemStack[] itemStacks)
    {
        for (int i = 0; i < slots.Length; i++)
            UpdateSlot(itemStacks, i);
    }

    /// <summary> Pass in null to make moveItemIcon invisible </summary>
    public void UpdateMoveItemIcon(ItemBase item)
    {
        SetImageSprite(ref moveItemIcon, item == null ? null : item.icon);
    }

    // Not implemented into UpdateSlot method
    private void SetImageSprite(ref Image _image, Sprite _sprite)
    {
        if (_sprite == null)
        {
            _image.color = Color.clear;
            _image.sprite = null;
        }
        else
        {
            _image.color = Color.white;
            _image.sprite = _sprite;
        }
    }
}
