using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventController : MonoBehaviour
{

    public PlayerController pc;
    public void SetPlayerInControl(int active)
    {
        if (active == 0)
            pc.InControl(false);
        else
            pc.InControl(true);
    }
}
