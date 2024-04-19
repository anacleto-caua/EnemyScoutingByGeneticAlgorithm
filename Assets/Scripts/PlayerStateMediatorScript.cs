using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMediatorScript : MonoBehaviour
{
    public void Captured()
    {
        if (gameObject.TryGetComponent(out AutomatedPlayerMovement autoPlayer) && autoPlayer.enabled)
        {
            autoPlayer.Captured();
        }
        else if (gameObject.TryGetComponent(out PlayerMovement player) && player.enabled)
        {
            player.Captured();
        }
        else
        {
            Debug.LogWarning("Line not suposed to be hit!!!!!");
        }
    }

    public void Escaped()
    {
        if (gameObject.TryGetComponent(out AutomatedPlayerMovement autoPlayer) && autoPlayer.enabled)
        {
            autoPlayer.Escaped();
        }
        else if(gameObject.TryGetComponent(out PlayerMovement player) && player.enabled)
        {
            player.Escaped();
        }
        else
        {
            Debug.LogWarning("Line not suposed to be hit!!!!!");
        }
    }
}
