using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScapeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(UnityEngine.Collider collider)
    {
        // If its not a player ignore!
        if (!collider.CompareTag("Player"))
        {
            return;
        }

        if (collider.TryGetComponent<PlayerMovement>(out var player))
        {
            // Player escaped!
            player.Escaped();
        }

    }
}
