using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public partial class GameManager : MonoBehaviour
{
    // Global variables
    private int dificulty { get; set; }
    private bool victory { get; set; }
    private float timeCount { get; set; }
    private float ghostKills { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dificulty = 0;
        victory = false;
        timeCount = 0;
        ghostKills = 0;
    }

    // Quit game
    public void QuitGame() { Application.Quit(); }

    // Load scene of given name, without destroying current GameManager
    public void LoadScene(string sceneName)
    {
        DontDestroyOnLoad(this);
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (UnityException e)
        {
            Debug.LogException(e);
        }
        switch (sceneName)
        {
            case "Game":
                StartCoroutine (gameCoroutine());
                break;
            default:
                break;
        }
    }
}

public partial class GameManager: MonoBehaviour
{
    // variables
    private int spheresCounter { get; set; }
    private bool enemyIsVulnerable { get; set; }
    private float enemySmallSpeed { get; set; }
    private float enemyBigSpeed { get; set; }
    private float enemySpawnInterval { get; set; }
    private float powerUpSpawnInterval { get; set; }
    private float powerUpDuration { get; set; }
    private float powerUpSlowDownDivider { get; set; }
    // components
    private GameObject spheresCounterGO { get; set; }
    private GameObject enemySpawnsGO { get; set; }
    private GameObject powerUpSpawnsGO { get; set; }
    private GameObject bunkerRoofGO { get; set; }
    private GameObject bunkerDoorGO { get; set; }

    // trigger interactions
    public void TouchSphere(GameObject sphere)
    {
        Destroy(sphere);
        spheresCounterGO.GetComponent<TMP_Text>().text = (--spheresCounter).ToString();
        if (spheresCounter <= 0) { StartCoroutine(bunkerCoroutine()); }
    }

    public void TouchEnemy(GameObject enemy)
    {
        if (enemyIsVulnerable)
        {
            Destroy (enemy);
        }
        else
        {
            victory = false;
            LoadScene("Finish");
        }
    }

    public void TouchPowerUp(GameObject powerUp)
    {
        Destroy(powerUp);
        StartCoroutine( powerUpEffectCoroutine() );
    }

    public void TouchFinishPlane()
    {
        StartCoroutine( finishCoroutine() );
    }

    // coroutines
    // initiate scene 'Game'
    private IEnumerator gameCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        // set variables
        spheresCounter = GameObject.Find("Spheres Parent").transform.childCount;
        enemyIsVulnerable = false;
        switch (dificulty)
        {
            case 0:     // easy
                enemySmallSpeed = 3.5f;
                enemyBigSpeed = 3.5f;
                enemySpawnInterval = 30.0f;
                powerUpSpawnInterval = 35.0f;
                powerUpDuration = 15.0f;
                powerUpSlowDownDivider = 3.0f;
                break;
            case 1:     // hard
                enemySmallSpeed = 0;
                enemyBigSpeed = 0;
                enemySpawnInterval = 0;
                powerUpSpawnInterval = 0;
                powerUpDuration = 0;
                powerUpSlowDownDivider = 0;
                break;
            default:
                break;
        }

        // get components
        spheresCounterGO = GameObject.Find("SpheresCounter (TMP)");
        spheresCounterGO.GetComponent<TMP_Text>().text = spheresCounter.ToString();
        enemySpawnsGO = GameObject.Find("Enemy Spawns");
        powerUpSpawnsGO = GameObject.Find("Item Spawns");
        bunkerDoorGO = GameObject.Find("Door");
        bunkerRoofGO = GameObject.Find("Roof");

        // start secondary coroutines
        StartCoroutine( enemiesSpawnCoroutine() );
        StartCoroutine( powerUpSpawnCoroutine() );
    }

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
    }

    // after gathering all spheres, open bunker
    private IEnumerator bunkerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        spheresCounterGO.gameObject.SetActive(false);
        bunkerDoorGO.gameObject.SetActive(false);
        bunkerRoofGO.gameObject.SetActive(false);
    }

    // when crossing checker line, finish the game
    private IEnumerator finishCoroutine()
    {
        victory = true;
        yield return new WaitForSeconds(0.5f);
        LoadScene("Finish");
    }
}