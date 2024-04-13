using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        Movement();
    }
    // public override void MoveLogic()
    // {
    //     if (Input.GetKey(KeyCode.W))
    //     {
    //         MoveUp();
    //     }
    //     if (Input.GetKey(KeyCode.S))
    //     {
    //         MoveDown();
    //     }
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         MoveLeft();
    //     }
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         MoveRight();
    //     }
    // }

    public float speed = 5.0f;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public override void MoveLogic()
    {
        //values between -1 and 1 if the player is pressing the keys 'a', 'd' or the arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        //values between -1 and 1 if the player is pressing the keys 'w', 's' or the arrow keys
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            //angle of the camera in the y axis
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smoothly rotate the player to the target angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //rotate the player
            transform.rotation = Quaternion.Euler(0, angle, 0);

            //set the move direction to the direction the camera is facing
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            //move the player
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }

    public void Captured()
    {
        Debug.Log("Ouch! They got me!");
    }

}
