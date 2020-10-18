using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public bool acceptAllTags;
    public ItemBase.Tag[] acceptedTags;

    [Header("Resources")]
    public Image icon;
    public Text amount;
    public Image active;

    [HideInInspector]
    public int slotIndex;            // Set OnAwake by the InventoryUI that manages this slot

    public delegate void OnPointerEnterEvent(int _slotIndex);
    public event OnPointerEnterEvent onPointerEnterEvent;
    public delegate void OnPointerExitEvent(int _slotIndex);
    public event OnPointerExitEvent onPointerExitEvent;
    public delegate void OnPointerDownEvent(int _slotIndex);
    public event OnPointerDownEvent onPointerDownEvent;
    public delegate void OnPointerUpEvent(int _slotIndex);
    public event OnPointerUpEvent onPointerUpEvent;

    // Check if this slot accepts a given tag
    public bool HasTag(ItemBase.Tag tag)
    {
        if (acceptAllTags) return true;
        for (int i = 0; i < acceptedTags.Length; i++) 
            if (tag == acceptedTags[i]) return true;
        return false;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (onPointerEnterEvent != null)
            onPointerEnterEvent.Invoke(slotIndex);
        active.enabled = true;
    }

    // POTENTIAL ISSUE
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (onPointerExitEvent != null)
            onPointerExitEvent.Invoke(-1);
        active.enabled = false;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (onPointerDownEvent != null)
            onPointerDownEvent.Invoke(slotIndex);
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (onPointerUpEvent != null)
            onPointerUpEvent.Invoke(slotIndex);
    }
}
