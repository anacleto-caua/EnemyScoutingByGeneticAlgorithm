using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CharacterMovement : MonoBehaviour
{

    public CharacterController controller;

    public float velocity;

    public float gravity;

    public Vector3 movement;

    // Start is called before the first frame update
    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        movement = controller.transform.position;

        velocity = 5f;
        gravity = -9.81f;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        if (controller.isGrounded)
        {
            MoveLogic();
            movement *= velocity* Time.deltaTime;
        }
        else
        {
            movement.y = gravity * Time.deltaTime;
        }

        controller.Move(movement);
        movement = Vector3.zero;
    }
    public abstract void MoveLogic();

    public void MoveUp()
    {
        movement += controller.transform.forward;
    }
    public void MoveDown()
    {
        movement += -controller.transform.forward;
    }
    public void MoveLeft()
    {
        movement += -controller.transform.right;
    }
    public void MoveRight()
    {
        movement += controller.transform.right;
    }
}
