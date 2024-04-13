using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : CharacterMovement
{
    public float radius = 7.0f;
    public GameObject sphere;

    public float viewAngle = 45f; // Half of the FOV
    public float viewDistance = 50f; // How far the enemy can see

    public LayerMask targetMask; // Layer to consider as targets

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

    }

    // Update is called once per frame
    public override void Update()
    {
        // Set the sphere's position to the center of the circle/sphere
        sphere.transform.position = transform.position;

        Movement();

        ViewAround();
        ViewAhead();

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
                        player.Captured();
                    }
                }
            }
        }
    }

}
