using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventController : MonoBehaviour
{

    public PlayerController pc;
    public Animator toDoAnim;
    public void SetPlayerInControl(int active)
    {
        if (active == 0)
            pc.InControl(false);
        else
            pc.InControl(true);
    }

    public void HideToDoList()
    {
        toDoAnim.SetBool("Hidden", true);
    }
    public void UnhideToDoList()
    {
        toDoAnim.SetBool("Hidden", false);
    }
}
