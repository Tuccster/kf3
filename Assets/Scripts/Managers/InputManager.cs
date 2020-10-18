using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public enum Input
    {
        Shoot,
        Reload,
        Interact,
        Inventory
    }

    private static KeyCode[] keys = 
    {
        KeyCode.Mouse0,
        KeyCode.R,
        KeyCode.E,
        KeyCode.Tab
    };

    public static KeyCode GetKey(Input input)
    {
        return keys[(int)input];
    }
}
