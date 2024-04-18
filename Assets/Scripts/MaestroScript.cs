using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaestroScript : MonoBehaviour
{

    #region AGParameters
    public int individuals;
    public int genes;
    public int roundsLimit;
    public float removePercent;
    public int mutantionChance;
    #endregion AGParameters

    #region AGStatistics
    public int rounds = 0;
    #endregion AGStatistics

    #region FunctionalVariables
    public Vector3 PlayerSpawnPoint;
    public PlayerMovement Player;

    GameObject EnemyPrefab;
    public List<Vector3> EnemySpawns;
    public List<List<EnemyMovement>> Enemies;

    public Vector3 HeroScapePosition;
    public HeroScapeScript HeroScape;

    #endregion FunctionalVariables

    // Awake is called before the any frame update
    void Awake()
    {
        individuals = 20;
        genes = 5;
        roundsLimit = 5;
        removePercent = 50;
        mutantionChance = 20;

        PlayerSpawnPoint = new Vector3(0f, 2f, 0f);
        HeroScapePosition = new Vector3(20f, 0f, 0f);

        EnemySpawns = new List<Vector3>();
        EnemySpawns.Add(new Vector3(12f, 2f, -1f));

        EnemyPrefab = Resources.Load<GameObject>("Enemy");
        
        Enemies = new List<List<EnemyMovement>>();


        // Generating enemies on the scene AND generating their initial path
        // For each enemy spawn run a for loop and add individuals Enemy to the Enemies array
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

        //Add one hero scape to the scene at the set spawn point
        GameObject HeroScapePrefab = Resources.Load<GameObject>("HeroScape");
        HeroScape = Instantiate(HeroScapePrefab, HeroScapePosition, Quaternion.identity).GetComponent<HeroScapeScript>();

        //Add an enemy barrier arround the hero scape
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.finished)
        {
            Debug.Log("Round " + rounds + " finished, re-ordering for the next round");
            rounds++;

            OrderEnemiesList();
            Debug.Log("Enemy 0 p: " + Enemies[0][0].score);
            Debug.Log("Enemy 1 p: " + Enemies[0][1].score);
            Debug.Log("Enemy 2 p: " + Enemies[0][2].score);
            Debug.Log("Enemy n p: " + Enemies[0].Last().score);

            // Override the low pontuation individuals with crosses of the individuals on list
            CrossToReplaceRemovedEnemies();

            RepositinateEntities();

            Player.finished = false;
        }
        
    }

    #region AGFunctions
    public void OrderEnemiesList()
    {
        foreach (List<EnemyMovement> enemyList in Enemies)
        {
            enemyList.Sort((x, y) => y.score.CompareTo(x.score));
        }
    }

    public void CrossToReplaceRemovedEnemies()
    {
        int cutFrom = (individuals / 100) * (100 - (int)removePercent);
        int count = 0; // Count the enemy spawns list id
        foreach (List<EnemyMovement> EnemyList in Enemies)
        {
            int ind = 0;
            int indCount = (EnemyList.Count % 2 == 0) ? EnemyList.Count : EnemyList.Count - 1;
            for (int i = cutFrom; i < individuals; i++)
            {
                if (ind >= indCount - 1)
                {
                    ind = 0;
                }
                
                EnemyList[i].Cross(EnemyList[ind], EnemyList[ind+1]);
                ind++;

                float mutationRoll = UnityEngine.Random.Range(1, 100);
                if(mutationRoll <= mutantionChance)
                {
                    EnemyList[i].Mutate();
                }
            }
            count++;
        }

    }

    public void RepositinateEntities()
    {
        // Reposition player
        Player.Repositionate(PlayerSpawnPoint);

        // Reposition enemies
        int count = 0;
        foreach (Vector3 spawn in EnemySpawns)
        {
            for (int i = 0; i < individuals; i++)
            {
                Enemies[count][i].ResetForNextRound(spawn);
            }
            count++;
        }
    }
    #endregion AGFunctions
}
