using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutomatedPlayerMovement : EnemyMovement
{
    public NavMeshAgent agent;
    public bool finished = false;
    public Vector3 HeroScape;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        agent.Move(HeroScape);
        //base.Update();

    }

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
        //Debug.Log("Ouch! They got me!!!");
    }

    public void Escaped()
    {
        finished = true;
        //Debug.Log("PlayerMovement escaped!!!");
    }
}
