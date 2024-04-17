using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaestroScript : MonoBehaviour
{

    #region AGParameters
    public int individuals;
    public int genes;
    public int roundsLimit;
    #endregion AGParameters

    #region FunctionalVariables
    public Vector3 PlayerSpawnPoint;
    public PlayerMovement Player;

    public List<Vector3> EnemySpawns;
    public List<List<EnemyMovement>> Enemies;

    public Vector3 HeroScapePosition;
    public HeroScapeScript HeroScape;

    #endregion FunctionalVariables

    // Start is called before the first frame update
    void Start()
    {
        individuals = 20;
        genes = 5;
        roundsLimit = 5;
        
        EnemySpawns.Add(new Vector3(12f, 2f, -1f));
        PlayerSpawnPoint = new Vector3(0f, 2f, 0f);
        HeroScapePosition = new Vector3(20f, 0f, 0f);
        Enemies = new List<List<EnemyMovement>>();

        // Generating enemies on the scene AND generating their initial path
        // For each enemy spawn run a for loop and add individuals Enemy to the Enemies array
        GameObject EnemyPrefab = Resources.Load<GameObject>("Enemy");
        Debug.Log("Generating enemies!");
        int count = 0;
        foreach (Vector3 spawn in EnemySpawns)
        {
            Enemies.Add(new List<EnemyMovement>());
            for(int i = 0; i < individuals; i++)
            {
                // Instantiate the enemy and adds his referent script to the list
                Enemies[count].Add(Instantiate(EnemyPrefab, EnemySpawns[count], Quaternion.identity).GetComponent<EnemyMovement>());
                Enemies[count][i].GenerateInitialPatrolPattern(genes);
            }
            count++;
        }

        //Add one player to the scene at the set spawn point
        GameObject PlayerPrefab = Resources.Load<GameObject>("Player");
        Player = Instantiate(PlayerPrefab, PlayerSpawnPoint, Quaternion.identity).GetComponent<PlayerMovement>();
        
        //Add an enemy barrier arround the spawn
        GameObject AntiEnemySphere = Resources.Load<GameObject>("AntiEnemySphere");
        Instantiate(AntiEnemySphere, PlayerSpawnPoint, Quaternion.identity);

        //Add one hero scape to the scene at the set spawn point
        GameObject HeroScapePrefab = Resources.Load<GameObject>("HeroScape");
        HeroScape = Instantiate(HeroScapePrefab, HeroScapePosition, Quaternion.identity).GetComponent<HeroScapeScript>();

        //Add an enemy barrier arround the hero scape
        Instantiate(AntiEnemySphere, HeroScapePosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
