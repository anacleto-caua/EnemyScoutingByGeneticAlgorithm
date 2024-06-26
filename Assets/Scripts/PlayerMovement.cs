using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : CharacterMovement
{
    public bool finished = false;

    public bool camPlay = true;

    public bool captured = false;

    public Camera PlayerCamera;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // Generate MaestroCamera for scene
        GameObject CameraPrefab = Resources.Load<GameObject>("Main Camera");
        PlayerCamera = Instantiate(PlayerCamera, transform.position, Quaternion.identity).GetComponent<Camera>();

        GameObject FreeLookCamera = Resources.Load<GameObject>("FreeLook Camera");
        FreeLookCamera = Instantiate(FreeLookCamera, transform.position, Quaternion.identity);

        CinemachineFreeLook FreeLook = FreeLookCamera.GetComponent<CinemachineFreeLook>();
        FreeLook.Follow = transform;
        FreeLook.LookAt = transform;

        cam = PlayerCamera.transform;
    }

    // Update is called once per frame  
    public override void Update()
    {
        if (camPlay)
        {
            Movement();
        
            PlayerRotation();
        }
    }


    #region PlayerRotationAndMovement

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

    public float speed = 16.0f;
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
            //angle of the MaestroCamera in the y axis
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smoothly rotate the player to the target angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //rotate the player
            transform.rotation = Quaternion.Euler(0, angle, 0);

            //set the move direction to the direction the MaestroCamera is facing
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            //move the player
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }

    bool rotateP90 = false;
    float currentPosition;
    bool isRotating = false;
    public float rotationSpeed = 1f;

    //rotate +90 degrees in the y axis
    //OBS.: this function only works with the rotationSpeed set to 1
    public bool RotateP90()
    {
        if (rotateP90)
        {
            currentPosition = transform.localRotation.eulerAngles.y;
            if(currentPosition + 90 >= 360){
                currentPosition = -90;
            }
            transform.Rotate(0, 0.5f, 0);
            isRotating = true;
            rotateP90 = false;
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
                rotateP90 = true;
                start360 = true;
            }
            if(!completedRotation)
            {
                bool result = RotateP90();
            
                if(result){
                    rotateP90 = true;
                    rotate360Aux++;
                }
                if(rotate360Aux == 4){
                    rotate360Aux = 0;
                    start360 = false;
                    completedRotation = true;
                }
            }else{
                rotateP90 = false;
                make360 = false;
                completedRotation = false;
                start360 = false;
            }
            
        }
    }

    public bool rotate90 = false;
    bool rotationAux = false;
    public void PlayerRotation()
    {
        if(make360){
            RotateP360();
        }else if(rotate90){
            if(!rotationAux){
                rotateP90 = true;
                rotationAux = true;
            }
            bool rotate = RotateP90();
            if(rotate){
                rotate90 = false;
                rotationAux = false;
            }
        }
    }

    #endregion PlayerRotationAndMovement

    public void Repositionate(Vector3 spawn)
    {
        // The character controller need to be disabled in order to perform thoose suddenly position transiotions
        controller.enabled = false;
        transform.position = spawn;
        controller.enabled = true;

        movement = Vector3.zero;
    }

    public void Captured()
    {
        captured = true;
        //Debug.Log("Ouch! They got me!!!");
    }

    public void Escaped()
    {
        finished = true;
        //Debug.Log("PlayerMovement escaped!!!");
    }

}
