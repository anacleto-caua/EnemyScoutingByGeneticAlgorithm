using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : CharacterMovement
{

    float radius = 3f;
    // public GameObject sphere;

    float viewAngle = 45f; // Half of the FOV
    float viewDistance = 50f; // How far the enemy can see

    public List<string> actions = new();
    public int idActions = 0;
    public bool reverseActions = false;
    public string[] possibleActions = { "move_front", "move_back", "move_right", "move_left", "look_around" };


    public int angleIncrease = 5;
    int targetAngleRotation = 0;

    Vector3 targetPosition;
    Vector3 positionBeforeLastMovement;

    float raycastDistance = 0.1f;  // The distance the raycast will check

    #region AGParameters
    int genes;
    bool scoredThisRound = false;
    public float score;
    #endregion AGParameters

    public void Awake()
    {
        targetPosition = controller.transform.position;

        /*
        #region SphereHitbox

        // Create the sphere
        // The sphere will be removed, but if you want try just draggin a normal sphere do this object and turn it public
        sphere = new GameObject("LookAroundSphere");

        // Add a SphereCollider to the GameObject
        sphere.AddComponent<SphereCollider>();

        // Add a MeshRenderer to the GameObject
        sphere.AddComponent<MeshRenderer>();

        // Create a sphere mesh
        MeshFilter meshFilter = sphere.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();

        // Set the sphere's scale based on the radius of the circle/sphere
        float diameter = radius * 2;
        sphere.transform.localScale = new Vector3(diameter, diameter, diameter);

        // Optionally, set the sphere's material, e.g. to make it semi-transparent
        Material material = new(Shader.Find("Transparent/Diffuse"))
        {
            color = new Color(1.0f, 0.0f, 0.0f, 0.2f) // Semi-transparent red
        };
        sphere.GetComponent<Renderer>().material = material;

        #endregion SphereHitbox        
        */

        velocity = 2.8f; 
        gravity = -9.81f;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        Gravity();

        // Makes the enemy score only once per round Currently: INACTIVE See: IsPlayerVisibleFromHere()
        if (!scoredThisRound)
        {
            ViewAround();
            ViewAhead();
        }

        if (targetAngleRotation == 0) {
            WallAvoider();

        }

        /**
         * Basically:
         * 
         * Is there a rotation necessary? Do it and finishes the update
         * Is there a movement necessary? Do it and finishes the update
         * Isn't any rotation or movement necessary? Call the next Action and finishes the update 
         * 
         * It counts trough the Action List one by one, when it hits the n, 
         * it activates "reverse mode" and runs the Action List from n to 0,
         * looping forever in this patrol pattern
         * 
         */
        if (targetAngleRotation != 0)
        {
            Rotate();
        }
        else if(controller.transform.position != targetPosition)
        {
            Movement();
        }
        else
        {
            if (idActions == actions.Count)
            {
                reverseActions = !reverseActions;
                idActions--;
            }
            if(idActions == -1)
            {
                reverseActions = !reverseActions;
                idActions++;
            }

            // Runs next action
            if (!reverseActions)
            {
                ExecuteAction(actions[idActions]);
                idActions++;
            }
            else
            {
                ExecuteAction(OpositeOfAction(actions[idActions]));
                idActions--;
            }
        }
    }

    #region AGBehaviourFunctions
    public void GenerateInitialPatrolPattern(int genes)
    {
        this.genes = genes;
        int randomIndex = 0;

        for (int i = 0; i < genes; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, possibleActions.Length);
            actions.Add(possibleActions[randomIndex]);
        }
    }

    public void Cross(EnemyMovement Father, EnemyMovement Mother)
    {
        for(int i = 0; i < genes; i++)
        {
            if(UnityEngine.Random.value > 0.5f)
            {
                actions[i] = Father.actions[i];
            }
            else
            {
                actions[i] = Mother.actions[i];
            }
        }

    }

    public void Mutate()
    {
        int randomIndex = UnityEngine.Random.Range(0, genes);
        int randomGene = UnityEngine.Random.Range(0, possibleActions.Length);
        actions[randomIndex] = possibleActions[randomGene];
    }

    public void ResetForNextRound(Vector3 spawPoint)
    {
        transform.position = spawPoint;
        positionBeforeLastMovement = spawPoint;
        targetPosition = spawPoint;

        targetAngleRotation = 0;
        transform.Rotate(Vector3.zero);

        idActions = 0;
        reverseActions = false;
    }
    #endregion AGBehaviourFunctions

    #region EnemyBehaviourFunctions
    public override void MoveLogic()
    {
    }
    public void ViewAround()
    {
        // Get all colliders within the circle/sphere
        Collider[] entities = Physics.OverlapSphere(transform.position, radius);

        // Loop through the colliders and do something with each one
        foreach (Collider target in entities)
        {
            if (!target.CompareTag("Player"))
            {
                continue;
            }

            // Is the player visible, or is him on the other side of the wall
            IsPlayerVisibleFromHere(target);
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
            
            // Is the player on the radius of view
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
            {
                // Is the player visible, or is him on the other side of the wall
                IsPlayerVisibleFromHere(target);
            }
        }
    }

    public bool IsPlayerVisibleFromHere(Collider target)
    {
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

        // Create a layer mask that includes all layers except the "EnemyBarrier" layer
        int layerMask = 1 << LayerMask.NameToLayer("EnemyBarrier");
        layerMask = ~layerMask;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, distanceToTarget, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (target.TryGetComponent<PlayerMovement>(out var player))
                {
                    // Score points for how many time he sees the enemy
                    score += Time.deltaTime;

                    // Uncoment the following line will make the enemy pontuate only once per round
                    // scoredThisRound = true;
                    player.Captured();
                }
                return true;
            }
        }
        return false;
    }
    public void ExecuteAction(string action)
    {
        float difference = 0.0f;
        switch (action)
        {
            case "look_around":
                targetAngleRotation = 360;
                
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
                difference = Mathf.DeltaAngle(transform.eulerAngles.y, -270);

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
                ExecuteAction("look_front");
                Vector3 targetRotation = Vector3.zero;
                targetPosition = controller.transform.position;
                targetPosition += controller.transform.forward * 3;
                positionBeforeLastMovement = controller.transform.position;

                break;
            case "move_back":
                ExecuteAction("look_back");
                targetPosition = controller.transform.position;
                targetPosition += controller.transform.forward * 3;
                positionBeforeLastMovement = controller.transform.position;

                break;
            case "move_left":
                ExecuteAction("look_left");
                targetPosition = controller.transform.position;
                targetPosition += controller.transform.forward * 3;
                positionBeforeLastMovement = controller.transform.position;

                break;
            case "move_right":
                ExecuteAction("look_right");
                targetPosition = controller.transform.position;
                targetPosition += controller.transform.forward * 3;
                positionBeforeLastMovement = controller.transform.position;

                break;

        }
    }
    public override void Movement()
    {
        /*
        */
        Vector3 rotationSnapping = transform.rotation.eulerAngles;
        rotationSnapping.y = RoundAngle(rotationSnapping.y);
        float dif = rotationSnapping.y - transform.rotation.eulerAngles.y;
        rotationSnapping = new Vector3(0, dif, 0);

        transform.Rotate(rotationSnapping);

        targetPosition = positionBeforeLastMovement + controller.transform.forward * 3;
        
        Gravity();
        
        controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetPosition, velocity * Time.deltaTime);
    }
    public void Gravity()
    {
        if (!controller.isGrounded)
        {
            Vector3 gr = Vector3.zero;
            gr.y = gravity * Time.deltaTime;
            controller.Move(gr);
            targetPosition.y = controller.transform.position.y;
            positionBeforeLastMovement.y = controller.transform.position.y;
        }
    }
    public void WallAvoider()
    {
        // Create a layer mask that includes all layers except the "Player" and "Enemy" layers
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Enemy"));
        layerMask = ~layerMask;

        // Cast a ray forward from this game object
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, raycastDistance, layerMask);

        foreach (RaycastHit hit in hits)
        {
            positionBeforeLastMovement = transform.position - transform.forward * 3;
            targetPosition = transform.position;
            break; // Stop checking after finding the first non-Enemy and non-Player object
        }
    }

    public void Rotate()
    {

        Vector3 eulers = new Vector3(0, 0, 0);

        if (targetAngleRotation % 5 != 0 && targetAngleRotation > Math.Abs(5))
        {
            eulers.y = targetAngleRotation;
            targetAngleRotation = 0;
        }

        if (targetAngleRotation > 0)
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
        int[] forbiddenAngles = { 0, 90, 180, 270, 360, -180, -90, -270 };
        int nearestForbiddenAngle = forbiddenAngles.OrderBy(x => Math.Abs((long)x - angle)).First();
        
        angle = nearestForbiddenAngle;
        
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

        if (action == "move_left")
        {
            return "move_right";
        }

        if (action == "move_right")
        {
            return "move_left";
        }

        if (action == "look_around")
        {
            return action;
        }

        Debug.LogWarning("Code not supposed to be hit!");
        return "warning";
    }
    #endregion EnemyBehaviourFunctions

}
