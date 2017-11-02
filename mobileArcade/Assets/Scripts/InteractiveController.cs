using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveController : MonoBehaviour
{
    public GameObject icon;
    public bool playerInRange = false;
    public DoorController doorController;

    void Start()
    {
        ShowIcon(false);
    }
    public void ShowIcon(bool active)
    {
        print(active);
        icon.SetActive(active);
        playerInRange = active;
    }

    public void Interact(PlayerController player)
    {
        if (doorController)
        {
            doorController.OpenDoor(player);
        }
    }
}