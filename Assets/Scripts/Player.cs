using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    public float interactReach;

    [Header("Resources")]
    public Inventory inventory;
    public Camera pCamera;
    public InventoryConfigHandler inventoryConfigHandler;

    void Update()
    {
        if (Input.GetKeyDown(InputManager.GetKey(InputManager.Input.Interact)))
        {
            RaycastHit hit;
            if (Physics.Raycast(pCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, interactReach))
            {
                if (hit.transform.GetComponent<IInteractable>() != null)
                    hit.transform.GetComponent<IInteractable>().Interact(transform);
            }
        }

        if (Input.GetKeyDown(InputManager.GetKey(InputManager.Input.Inventory)))
            inventoryConfigHandler.OpenConfig(inventory);
    }
}
