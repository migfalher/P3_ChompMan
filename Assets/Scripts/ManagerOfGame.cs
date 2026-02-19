using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public partial class ManagerOfGame: MonoBehaviour
{
    // variables
    private bool enemyIsVulnerable;
    private float enemySmallSpeed;
    private float enemyBigSpeed;
    private float enemySpawnInterval;
    private float powerUpSpawnInterval;
    private float powerUpDuration;
    private float powerUpSlowDownDivider;
    // components
    private GameObject enemySpawnsGO;
    private GameObject powerUpSpawnsGO;
    private GameObject bunkerRoofGO;
    private GameObject bunkerDoorGO;
    private TimeCounter timeCounter;
    // ui elements
    private TMP_Text spheresCounterTMP;
    private TMP_Text ghostCounterTMP;
    private TMP_Text killsCounterTMP;

    // getters and setters
    public float getEnemySmallSpeed() { return enemySmallSpeed; }
    public float getEnemyBigSpeed() { return enemyBigSpeed; }
    public float getPowerUpSlowDowdDivider() { return powerUpSlowDownDivider; }

    // Awake
    private void Awake()
    {
        // search for components
        enemySpawnsGO = GameObject.Find("Enemy Spawns");
        powerUpSpawnsGO = GameObject.Find("Item Spawns");
        bunkerRoofGO = GameObject.Find("Roof");
        bunkerDoorGO = GameObject.Find("Door");
        spheresCounterTMP = GameObject.Find("SpheresCounter (TMP)").GetComponent<TMP_Text>();
        ghostCounterTMP = GameObject.Find("GhostCounter (TMP)").GetComponent<TMP_Text>();
        killsCounterTMP = GameObject.Find("KillsCounter (TMP)").GetComponent<TMP_Text>();
        timeCounter = this.AddComponent<TimeCounter>();
    }

    // Start
    private void Start()
    {
        // initialize variables
        GlobalVariables.fulfillSpheresCounter(GameObject.Find("Spheres Parent").transform.childCount);
        timeCounter.setTimeCounterTMP(GameObject.Find("TimeCounter (TMP)").GetComponent<TMP_Text>());
        spheresCounterTMP.text = GlobalVariables.getSpheresCounter().ToString();
        
        // set dificulty
        switch (GlobalVariables.getDificulty())
        {
            case 1:     // easy
                enemySmallSpeed = 6.0f;
                enemyBigSpeed = 3.0f;
                enemySpawnInterval = 30.0f;
                powerUpSpawnInterval = 5.0f;
                powerUpDuration = 15.0f;
                powerUpSlowDownDivider = 5.0f;
                break;
            case 2:     // hard
                enemySmallSpeed = 10.0f;
                enemyBigSpeed = 6.0f;
                enemySpawnInterval = 15.0f;
                powerUpSpawnInterval = 10.0f;
                powerUpDuration = 5.0f;
                powerUpSlowDownDivider = 2.0f;
                break;
            default:
                break;
        }
        
        // start secondary coroutines
        StartCoroutine(enemiesSpawnCoroutine());
        StartCoroutine(powerUpSpawnCoroutine());
        timeCounter.setTimeCounterIsOn(true);
    }

    // Update
    private void Update()
    {
        // update 'ghostCounter'
        int ghostCount = GlobalVariables.getGhostCounter();
        GlobalVariables.updateGhostCounter(GameObject.FindGameObjectsWithTag("Enemy").Length);
        ghostCounterTMP.text = (ghostCount < 10) ? ("0" + ghostCount.ToString()) : ghostCount.ToString();
    }

    // trigger interactions
    public void TouchSphere(GameObject sphere)
    {
        sphere.SetActive(false);
        GlobalVariables.subtractSpheresCounter();
        if (GlobalVariables.getSpheresCounter() <= 0)
        {
            StartCoroutine(bunkerCoroutine());
        }
        else
        {
            spheresCounterTMP.text = (GlobalVariables.getSpheresCounter()).ToString();
        }
    }

    public void TouchEnemy(GameObject enemy)
    {
        if (enemyIsVulnerable)
        {
            enemy.SetActive(false);
            ghostCounterTMP.text = GlobalVariables.getGhostCounter().ToString();
            GlobalVariables.addKillsCounter();
            killsCounterTMP.text = (GlobalVariables.getKillsCounter() < 10) ? ("0" + GlobalVariables.getKillsCounter().ToString()) : GlobalVariables.getKillsCounter().ToString();
        }
        else
        {
            // colocar en corrutina para pausa de 0.5 segundos
            StartCoroutine( looseCoroutine() );
        }
    }

    public void TouchPowerUp(GameObject powerUp)
    {
        powerUp.SetActive(false);
        StartCoroutine( powerUpEffectCoroutine() );
    }

    public void TouchChecker()
    {
        StartCoroutine( checkerCoroutine() );
    }

    // coroutines
    // spawn enemies
    private IEnumerator enemiesSpawnCoroutine()
    {
        // initiate variables
        Random rand = new Random();
        GameObject enemySmallGO = Resources.Load("Enemy_Small").GameObject();
        GameObject enemyBigGO = Resources.Load("Enemy_Big").GameObject();
        int spawnCount = enemySpawnsGO.transform.childCount;
        int spawnIndex_Small = 0;
        int spawnIndex_Big = 0;

        // start spawn loop
        while (!enemyIsVulnerable)
        {
            // set random positions
            spawnIndex_Small = rand.Next(0, spawnCount);
            do { spawnIndex_Big = rand.Next(0, spawnCount); } while (spawnIndex_Small == spawnIndex_Big);
            enemySmallGO.transform.position = enemySpawnsGO.transform.GetChild(spawnIndex_Small).transform.position;
            enemyBigGO.transform.position = enemySpawnsGO.transform.GetChild(spawnIndex_Big).transform.position;
            // instantiate enemies
            Instantiate(enemySmallGO, enemySmallGO.transform.position, Quaternion.identity);
            Instantiate(enemyBigGO, enemyBigGO.transform.position, Quaternion.identity);
            // wait for interval
            yield return new WaitForSeconds(enemySpawnInterval);
        }

        // wait until enemyIsVulnerable turns to 'false' before restarting the coroutine
        yield return new WaitUntil(() => !enemyIsVulnerable);
        StartCoroutine( enemiesSpawnCoroutine() );
    }

    // spawn powerUps
    private IEnumerator powerUpSpawnCoroutine()
    {
        // initiate variables
        Random rand = new Random();
        GameObject powerUpGO = Resources.Load("PowerUp").GameObject();
        int spawnCount = powerUpSpawnsGO.transform.childCount;
        int spawnIndex = rand.Next(0, spawnCount);

        // wait to spawn powerUp
        yield return new WaitForSeconds(powerUpSpawnInterval);
        powerUpGO.transform.position = powerUpSpawnsGO.transform.GetChild(spawnIndex).position;
        Instantiate (powerUpGO, powerUpGO.transform.position, Quaternion.identity);

        // restart coroutine
        yield return new WaitUntil(() => enemyIsVulnerable);
        yield return new WaitUntil(() => !enemyIsVulnerable);
        StartCoroutine( powerUpSpawnCoroutine() );
    }

    // execute powerUp effect
    private IEnumerator powerUpEffectCoroutine()
    {
        enemyIsVulnerable = true;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyNavigation>().setTargetTag("Hideout");
        }

        yield return new WaitForSeconds(powerUpDuration);

        enemyIsVulnerable = false;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyNavigation>().setTargetTag("Player");
        }
    }

    // if touched an enemy when enemyIsVulnerable == false, you loose
    private IEnumerator looseCoroutine()
    {
        timeCounter.setTimeCounterIsOn(false);
        GlobalVariables.setTimeText(timeCounter.getTimeCounterText());
        GlobalVariables.setVictory(false);
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Finish");
    }

    // after gathering all spheres, open bunker
    private IEnumerator bunkerCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        spheresCounterTMP.gameObject.SetActive(false);
        bunkerDoorGO.gameObject.SetActive(false);
        bunkerRoofGO.gameObject.SetActive(false);
    }

    // when crossing checker line, finish the game
    private IEnumerator checkerCoroutine()
    {
        GlobalVariables.setVictory(true);
        timeCounter.setTimeCounterIsOn(false);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Finish");
    }
}