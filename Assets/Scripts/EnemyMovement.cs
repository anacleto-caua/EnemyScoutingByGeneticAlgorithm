using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : CharacterMovement
{

    public float radius = 7.0f;
    public GameObject sphere;

    public float viewAngle = 45f; // Half of the FOV
    public float viewDistance = 50f; // How far the enemy can see

    public List<string> actions;
    public bool lastActionFinished;
    public int idActions = 0;
    public bool reverseActions = false;

    public float a = 0f;
    public float b = 0f;

    public float targetAngle = 0;
    public int angleIncrease = 5;
    public int targetAngleRotation = 0;
    // clockwise or counterclockwise
    public string rotationDirection = "clockwise";

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        #region SphereHitbox
        // Set the sphere's position to the center of the circle/sphere
        sphere.transform.position = transform.position;

        // Set the sphere's scale based on the radius of the circle/sphere
        float diameter = radius * 2;
        sphere.transform.localScale = new Vector3(diameter, 0.001f, diameter);

        // Optionally, set the sphere's material, e.g. to make it semi-transparent
        Material material = new(Shader.Find("Transparent/Diffuse"))
        {
            color = new Color(1.0f, 0.0f, 0.0f, 0.2f) // Semi-transparent red
        };
        sphere.GetComponent<Renderer>().material = material;

        #endregion SphereHitbox

        actions = new List<string>();
        actions.Add("move_front");
        actions.Add("move_front");
        actions.Add("move_front");
        actions.Add("move_front");
        actions.Add("look_around");
    }

    // Update is called once per frame
    public override void Update()
    {
        Movement();
        ViewAround();
        ViewAhead();

        if (!reverseActions)
        {
            ExecuteAction(actions[idActions]);
        }
        else
        {
            ExecuteAction(OpositeOfAction(actions[idActions]));
        }

        
        if (targetAngleRotation != 0)
        {
            Rotate();
        }


        if (lastActionFinished)
        {
            idActions++;
            if (idActions >= actions.Count)
            {
                idActions = 0;
                reverseActions = !reverseActions;
            }
        }


        // Update the sphere's position and size if necessary
        sphere.transform.position = transform.position;
        float diameter = radius * 2;
        sphere.transform.localScale = new Vector3(diameter, diameter, diameter);
    }

    public override void MoveLogic()
    {
        
    }
    
    public void ViewAround()
    {
        // Get all colliders within the circle/sphere
        Collider[] entities = Physics.OverlapSphere(transform.position, radius);

        // Loop through the colliders and do something with each one
        foreach (Collider entity in entities)
        {
            if (!entity.CompareTag("Player")) {
                continue;
            }

            if (entity.TryGetComponent<PlayerMovement>(out var player))
            {
                player.Captured();
            }

        }
    }

    public void ViewAhead()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (Collider target in targetsInViewRadius)
        {
            if (!target.CompareTag("Player"))
            {
                continue;
            }
            
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                Vector3 RaycastOrigin = transform.position + (transform.forward * 2);
                if (!Physics.Raycast(RaycastOrigin, dirToTarget, distanceToTarget))
                {
                    if (!target.CompareTag("Player"))
                    {
                        continue;
                    }

                    if (target.TryGetComponent<PlayerMovement>(out var player))
                    {
                        Debug.Log("Player, found!");
                        player.Captured();
                    }
                }
            }
        }
    }

    public void ExecuteAction(string action)
    {
        float difference = 0.0f;
        switch (action)
        {
            case "look_around":
                lastActionFinished = false;
                if(targetAngleRotation <= 5)
                {
                    lastActionFinished = true;
                    targetAngleRotation = 360;
                }
                
                break;
            case "look_front":
                difference = Mathf.DeltaAngle(transform.eulerAngles.y, 0);

                if (Mathf.Abs(difference) < 0.005f)
                {
                    // Finished the end of the rotation
                    transform.Rotate(new Vector3(0, difference, 0));
                }
                else 
                {
                    targetAngleRotation = RoundAngle(difference);
                }
                break;
            case "look_back":
                difference = Mathf.DeltaAngle(transform.eulerAngles.y, 180);

                if (Math.Abs(difference) < 0.005f)
                {
                    // Finished the end of the rotation
                    transform.Rotate(new Vector3(0, difference, 0));
                }
                else
                {
                    targetAngleRotation = RoundAngle(difference);
                }
                break;
            case "look_right":
                difference = Mathf.DeltaAngle(transform.eulerAngles.y, 90);

                if (Mathf.Abs(difference) < 0.005f)
                {
                    // Finished the end of the rotation
                    transform.Rotate(new Vector3(0, difference, 0));
                }
                else
                {
                    targetAngleRotation += RoundAngle(difference);
                }
                break;
            case "look_left":
                difference = Mathf.DeltaAngle(transform.eulerAngles.y, 270);

                if (Math.Abs(difference) < 0.005f)
                {
                    // Finished the end of the rotation
                    transform.Rotate(new Vector3(0, difference, 0));
                }
                else
                {
                    targetAngleRotation += RoundAngle(difference);
                }
                break;
            case "move_front":
                if(targetAngleRotation <= 5 && lastActionFinished)
                {
                    ExecuteAction("look_front");
                    lastActionFinished = false;
                }
                if(targetAngleRotation <= 5)
                {
                    MoveUp();
                    lastActionFinished = true;
                }
                break;
            case "move_back":
                if(targetAngleRotation <= 5 && lastActionFinished) { 
                    ExecuteAction("look_back");
                    lastActionFinished = false;
                }
                if (targetAngleRotation <= 5)
                {
                    MoveUp();
                    lastActionFinished = true;
                }
                break;

        }
    }

    public void Rotate()
    {

        Vector3 eulers = new Vector3(0, 0, 0);

        if(targetAngleRotation % 5 != 0){
            eulers.y = targetAngleRotation;
            targetAngleRotation = 0;
        }

        if(targetAngleRotation > 0)
        {
            eulers.y = angleIncrease;
            targetAngleRotation -= angleIncrease;
        }
        else if(targetAngleRotation < 0)
        {
            eulers.y = -angleIncrease;
            targetAngleRotation += angleIncrease;
        }
        
        transform.Rotate(eulers);
    }
    public int RoundAngle(float angle)
    {
        float[] forbiddenAngles = { 0f, 90f, 180f, 270f, 360f , -180f, -90f};
        float nearestForbiddenAngle = forbiddenAngles.OrderBy(x => Math.Abs((long)x - angle)).First();
        if (Math.Abs(nearestForbiddenAngle - angle) > 0) // if the difference is less than 1 degree
        {
            while (angle < nearestForbiddenAngle)
            {
                angle += 1; // increase the angle if it's less than the forbidden angle
            }
         
            while (angle > nearestForbiddenAngle)
            {
                angle -= 1; // decrease the angle if it's more than the forbidden angle
            }
        }
        return (int)angle;
    }

    public string OpositeOfAction(string action)
    {
        if(action == "move_front")
        {
            return "move_back";
        }

        if (action == "move_back")
        {
            return "move_front";
        }

        if (action == "look_around")
        {
            return action;
        }

        Debug.LogWarning("Code not supposed to be hit!");
        return "warning";
    }
}
