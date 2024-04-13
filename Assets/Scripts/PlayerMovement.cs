using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public override void MoveLogic()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveUp();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDown();
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
    }
}
