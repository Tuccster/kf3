using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    public bool updateOnAwake;
    public InventoryConfigHandler.InventoryType inventoryType;

    [Header("Resources")]
    public InventoryConfigHandler inventoryConfigHandler;
    public ItemDropped defaultDroppedItem;

    [HideInInspector]
    public ItemStack[] itemStacks;
    private IEnumerator addItemCoroutine;

    private InventoryUI inventoryUI;

    // Used for moving around items
    private int curActiveSlot;
    private int fromIndex;

    public class ItemStack
    {
        public ItemBase item;
        public int amount;

        public ItemStack(ItemBase _item = null, int _amount = 0)
        {
            item = _item;
            amount = _amount;
        }
    }

    private void Start()
    {
        inventoryUI = inventoryConfigHandler.GetInventoryType(inventoryType);

        itemStacks = new ItemStack[inventoryUI.slots.Length];
        for (int i = 0; i < itemStacks.Length; i++)
        {
            itemStacks[i] = new ItemStack(null, 0);
            inventoryUI.slots[i].onPointerDownEvent += OnSlotPointerDown;
            inventoryUI.slots[i].onPointerUpEvent += OnSlotPointerUp;
            inventoryUI.slots[i].onPointerEnterEvent += SetCurrentMouseOverSlot;
            inventoryUI.slots[i].onPointerExitEvent += SetCurrentMouseOverSlot;
        }

        if (updateOnAwake) inventoryUI.UpdateAllSlots(itemStacks);
    }

    public void DropItem(ItemBase item, int amount)
    {
        GameObject droppedItemObject = item.droppedModel == null ? defaultDroppedItem.gameObject : item.droppedModel.gameObject;
        GameObject newDroppedItem = GameObject.Instantiate(droppedItemObject, transform.position, Quaternion.identity);
        newDroppedItem.GetComponent<ItemDropped>().item = item;
        newDroppedItem.GetComponent<ItemDropped>().amount = amount;
    }

    public bool ItemStackEmpty(int _index)
    {
        return itemStacks[_index].item == null;
    }

    public void ClearItemStack(int _index)
    {
        itemStacks[_index].item = null;
        itemStacks[_index].amount = 0;
    }

    // Starts AddItemCoroutine() if it isn't already started
    public void AddItem(ItemBase item, int amount)
    {
        if (addItemCoroutine != null) return;
        addItemCoroutine = AddItemCoroutine(item, amount);
        StartCoroutine(addItemCoroutine);
    }

    private IEnumerator AddItemCoroutine(ItemBase item, int amount)
    {
        byte searchIterations = 0;
        while(amount > 0)
        {
            // Search for slot of same item type and that isn't full
            bool matchingItemFound = false;
            for (int i = 0; i < itemStacks.Length; i++)
            {
                if (itemStacks[i].item == null || itemStacks[i].item.id != item.id || itemStacks[i].amount >= itemStacks[i].item.stackSize) continue;
                if (!inventoryUI.slots[i].HasTag(item.tag)) continue;

                matchingItemFound = true;
                AddAmountToItemStack(i, ref amount);
                break;
            }

            // If a matching item slot wasn't found, find a empty slot to populate
            bool emptySlotFound = false;
            if (!matchingItemFound)
            {
                for (int i = 0; i < itemStacks.Length; i++)
                {
                    if (itemStacks[i].item != null) continue;
                    if (!inventoryUI.slots[i].HasTag(item.tag)) continue;
                    
                    itemStacks[i].item = item;
                    AddAmountToItemStack(i, ref amount);
                    emptySlotFound = true;
                    break;
                }
            }

            // If a matching or empty slot wasn't found, drop the remaining items
            if (!emptySlotFound && !matchingItemFound) 
            {
                DropItem(item, amount);
                amount = 0;
            }

            // Exit AddItemCoroutine() at 255 iterations
            searchIterations++;
            if (searchIterations == byte.MaxValue)
            {
                Debug.LogError($"Search iterations reached limit ({byte.MaxValue})");
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        inventoryUI.UpdateAllSlots(itemStacks);
        addItemCoroutine = null;
    }

    // Used exclusively witin AddItemCoroutine()
    private void AddAmountToItemStack(int stackIndex, ref int amountRemaining)
    {
        int maxAmountToAdd = itemStacks[stackIndex].item.stackSize - itemStacks[stackIndex].amount;
        if (maxAmountToAdd < amountRemaining)
        {
            itemStacks[stackIndex].amount += maxAmountToAdd;
            amountRemaining -= maxAmountToAdd;
        }
        else
        {
            itemStacks[stackIndex].amount += amountRemaining;
            amountRemaining = 0;
        }
    }

    private void OnSlotPointerDown(int _slotIndex)
    {
        fromIndex = _slotIndex;
        if (!ItemStackEmpty(_slotIndex))
            inventoryUI.UpdateMoveItemIcon(itemStacks[_slotIndex].item);
    }

    // This is where items get switched between slots
    private void OnSlotPointerUp(int _slotIndex)
    {
        inventoryUI.UpdateMoveItemIcon(null);
        if (curActiveSlot == fromIndex) return;

        if (curActiveSlot == -1)
        {
            DropItem(itemStacks[fromIndex].item, itemStacks[fromIndex].amount);
            ClearItemStack(fromIndex);
            inventoryUI.UpdateAllSlots(itemStacks);
            return;
        }

        if (itemStacks[curActiveSlot].item == itemStacks[fromIndex].item)
            AddAmountToItemStack(curActiveSlot, ref itemStacks[fromIndex].amount);
        else
        {
            ItemStack swapStack = new ItemStack(itemStacks[curActiveSlot].item, itemStacks[curActiveSlot].amount);
            itemStacks[curActiveSlot] = new ItemStack(itemStacks[fromIndex].item, itemStacks[fromIndex].amount);
            itemStacks[fromIndex] = new ItemStack(swapStack.item, swapStack.amount);
        }
        inventoryUI.UpdateAllSlots(itemStacks);
    }

    private void SetCurrentMouseOverSlot(int _slotIndex)
    {
        curActiveSlot = _slotIndex;
    }

    private void PrintContents()
    {
        for (int i = 0; i < itemStacks.Length; i++)
            Debug.Log($"stack={i} item={itemStacks[i].item} amount={itemStacks[i].amount}");
    }
}
