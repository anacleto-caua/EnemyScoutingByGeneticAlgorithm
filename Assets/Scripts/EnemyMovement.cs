using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMovement
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
    public override void MoveLogic()
    {
        
    }
}
