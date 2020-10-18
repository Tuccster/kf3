using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/ItemBase", order = 0)]
public class ItemBase : ScriptableObject
{
    public string displayName = "New Item";
    public string id = "new_item";
    public Sprite icon = null;
    [Range(1, 1000)]
    public int stackSize = 1;
    [Tooltip("If not set, system will use default.")]
    public ItemDropped droppedModel;

    public enum Tag 
    { 
        Untagged,
        Weapon,
        Ammo,
        Material
    }
    public Tag tag;
}
