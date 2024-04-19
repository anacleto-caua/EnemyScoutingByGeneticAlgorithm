using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    public PlayerMovement PlayerMovement;
    public AutomatedPlayerMovement PlayerAutoMovement;

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

        // Commented because I'm inserting the data on the editor window
        //PlayerSpawnPoint = new Vector3(0f, 2f, 0f);
        //HeroScapePosition = new Vector3(20f, 0f, 0f);

        //EnemySpawns = new List<Vector3>();
        //EnemySpawns.Add(new Vector3(12f, 2f, -1f));

        EnemyPrefab = Resources.Load<GameObject>("Enemy");
        
        Enemies = new List<List<EnemyMovement>>();


        // Generating enemies on the scene AND generating their initial path
        // For each enemy spawn run a for loop and add individuals Enemy to the Enemies array
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
        GameObject Player = Instantiate(PlayerPrefab, PlayerSpawnPoint, Quaternion.identity);
        PlayerMovement = Player.GetComponent<PlayerMovement>();
        PlayerAutoMovement = Player.GetComponent<AutomatedPlayerMovement>();
        PlayerAutoMovement.HeroScape = HeroScapePosition;
        #region PlayerActionsSetManually
        PlayerAutoMovement.actions.Add("move_back");
        PlayerAutoMovement.actions.Add("move_back");
        PlayerAutoMovement.actions.Add("move_back");
        PlayerAutoMovement.actions.Add("move_back");
        PlayerAutoMovement.actions.Add("move_back");

        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");

        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");

        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");
        PlayerAutoMovement.actions.Add("move_right");

        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");
        PlayerAutoMovement.actions.Add("move_front");

        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        PlayerAutoMovement.actions.Add("move_left");
        #endregion PlayerActionsSetManually

        DisablePlayerMovement();
        EnablePlayerAutomatedMovement();

        //Add an enemy barrier arround the spawn
        //GameObject AntiEnemySphere = Resources.Load<GameObject>("AntiEnemySphere");

        //Add one hero scape to the scene at the set spawn point
        GameObject HeroScapePrefab = Resources.Load<GameObject>("HeroScape");
        HeroScape = Instantiate(HeroScapePrefab, HeroScapePosition, Quaternion.identity).GetComponent<HeroScapeScript>();

        //Add an enemy barrier arround the hero scape
        // .......................

        // Set time scale high so the training will be faster
        Time.timeScale = 5;
    }


    public bool playerWillPlayNextRound = false;
    void Update()
    {
        if (PlayerMovement.finished && playerWillPlayNextRound)
        {
            OrderEnemiesList();
            if (Enemies[0][0].score > 0)
            {
                Debug.Log("Foi capturado!");
            }
            playerWillPlayNextRound = false;
            PlayerAutoMovement.finished = true;
            EnablePlayerAutomatedMovement();
            DisablePlayerMovement();
            
        }
        
        if(PlayerAutoMovement.finished)
        {
            Debug.Log("Round " + rounds + " finished, re-ordering for the next round");
            rounds++;

            OrderEnemiesList();
            Debug.Log("00 enemy score: " + Enemies[0][0].score);
            Debug.Log("01 enemy score: " + Enemies[0][1].score);
            Debug.Log("02 enemy score: " + Enemies[0][2].score);
            Debug.Log("Last enemy score: " + Enemies[0].Last().score);

            // Override the low pontuation individuals with crosses of the individuals on list
            CrossToReplaceRemovedEnemies();

            RepositinateEntities();

            PlayerAutoMovement.finished = false;

            if (playerWillPlayNextRound)
            {
                // Set time scale to default so player can feel ok playing
                Time.timeScale = 1; 
                DisablePlayerAutomatedMovement();
                EnablePlayerMovement();
            }
            else
            {
                // Set time scale high so the training will be faster
                Time.timeScale = 5;
            }
        }

        // If the user press the L key he can play on the next round
        if (Input.GetKey(KeyCode.L))
        {
            playerWillPlayNextRound = true;
            Debug.Log("You gonna play when this round finishes!");
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
        int cutFrom = individuals * (100 - (int)removePercent) / 100;
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
        PlayerMovement.Repositionate(PlayerSpawnPoint);
        PlayerAutoMovement.Repositionate(PlayerSpawnPoint);
        PlayerAutoMovement.ResetForNextRound(PlayerSpawnPoint);

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

    #region Disable&EnablePlayerScripts
    public void DisablePlayerMovement() { 
        //PlayerMovement.GetComponent<PlayerMovement>().enabled = false;
        PlayerMovement.GetComponent<PlayerMovement>().camPlay = false;
    }
    public void DisablePlayerAutomatedMovement() {
        PlayerMovement.GetComponent<AutomatedPlayerMovement>().enabled = false;
    }
    public void EnablePlayerMovement()
    {
        //PlayerMovement.GetComponent<PlayerMovement>().enabled = true;
        PlayerMovement.GetComponent<PlayerMovement>().camPlay= true;
    }
    public void EnablePlayerAutomatedMovement()
    {
        PlayerMovement.GetComponent<AutomatedPlayerMovement>().enabled = true;
    }
    #endregion Disable&EnablePlayerScripts

}
