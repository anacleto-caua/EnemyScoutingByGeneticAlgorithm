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
        //RotateP90();
        RotateP360();
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

    public float speed = 8.0f;
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

    public int rotateP = 0;
    float currentPosition;
    bool isRotating = false;
    public float rotationSpeed = 1f;

    //rotate +90 degrees in the y axis
    //OBS.: this function only works with the rotationSpeed set to 1
    public bool RotateP90()
    {
        if (rotateP == 1)
        {
            currentPosition = transform.localRotation.eulerAngles.y;
            if(currentPosition + 90 > 360){
                currentPosition = -90;
            }
            transform.Rotate(0, 0.5f, 0);
            isRotating = true;
            rotateP = 0;
        }else if(isRotating){
            
            if (transform.localRotation.eulerAngles.y < currentPosition + 90 || (currentPosition == -90 && transform.localRotation.eulerAngles.y > currentPosition + 91))
            {
                transform.Rotate(0, rotationSpeed, 0);
            }else{
                isRotating = false;
                return true;
            }
        }
        return false;
    }

    int rotate360Aux = 0;
    bool completedRotation = false;
    public bool make360 = false;
    bool start360 = false;
    public void RotateP360()
    {
        if(make360){
            if(!start360){
                rotateP = 1;
                start360 = true;
            }
            if(!completedRotation)
            {
                bool result = RotateP90();
            
                if(result){
                    rotateP = 1;
                    rotate360Aux++;
                }
                if(rotate360Aux == 4){
                    rotate360Aux = 0;
                    start360 = false;
                    completedRotation = true;
                }
            }else{
                make360 = false;
                completedRotation = false;
            }
            
        }
    }

    public void Captured()
    {
        Debug.Log("Ouch! They got me!");
    }

}
