using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public partial class GameManager : MonoBehaviour
{
    // didn't know how to get this programatically
    public GameObject panel;

    // Global variables
    private int dificulty { get; set; }
    private bool victory { get; set; }
    private TimeCounter timeCounter { get; set; }
    private float ghostKills { get; set; }

    public void setDificulty(int _dificulty) { dificulty = _dificulty; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dificulty = -1;
        victory = false;
        timeCounter = this.gameObject.GetComponent<TimeCounter>();
        ghostKills = 0;
    }

    // Quit game
    public void QuitGame() { Application.Quit(); }

    // activate dificulty panel
    public void dificultyPanel()
    {
        panel.SetActive(true);
        StartCoroutine( LoadGame() );
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitUntil(() => dificulty > -1);
        LoadScene("Game");
        StartCoroutine(gameCoroutine());
    }

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
                StartCoroutine( gameCoroutine() );
                break;
            case "Finish":
                StartCoroutine( finishCoroutine() );
                break;
            case "Menu":
                dificulty = -1;
                victory = false;
                ghostKills = 0;
                break;
            case "Credits":
                StartCoroutine( creditsCoroutine() );
                break;
            default:
                break;
        }
    }
}

public partial class GameManager: MonoBehaviour
{
    // variables
    private int spheresCounter;
    private int ghostCounter;
    private int killsCounter;
    private bool enemyIsVulnerable;
    private float enemySmallSpeed;
    private float enemyBigSpeed;
    private float enemySpawnInterval;
    private float powerUpSpawnInterval;
    private float powerUpDuration;
    private float powerUpSlowDownDivider;
    // components
    private GameObject spheresCounterGO;
    private GameObject enemySpawnsGO;
    private GameObject powerUpSpawnsGO;
    private GameObject bunkerRoofGO;
    private GameObject bunkerDoorGO;
    // ui elements
    private TMP_Text ghostCounterTMP;
    private TMP_Text killsCounterTMP;

    // getters and setters
    public float getEnemySmallSpeed() { return enemySmallSpeed; }
    public float getEnemyBigSpeed() { return enemyBigSpeed; }
    public float getPowerUpSlowDowdDivider() { return powerUpSlowDownDivider; }

    // trigger interactions
    public void TouchSphere(GameObject sphere)
    {
        if (spheresCounterGO == null)
        {
            spheresCounterGO = GameObject.Find("SpheresCounter (TMP)");
        }
        sphere.SetActive(false);
        spheresCounterGO.GetComponent<TMP_Text>().text = (--spheresCounter).ToString();
        if (spheresCounter <= 0) { StartCoroutine(bunkerCoroutine()); }
    }

    public void TouchEnemy(GameObject enemy)
    {
        if (ghostCounterTMP == null)
        {
            ghostCounterTMP = GameObject.Find("GhostCounter (TMP)").GetComponent<TMP_Text>();
        }
        if (enemyIsVulnerable)
        {
            Destroy (enemy);
            ghostCounter = (ghostCounter < 0) ? 0 : ghostCounter - 1;
            ghostCounterTMP.text = (ghostCounter < 10) ? ("0" + ghostCounter.ToString()) : ghostCounter.ToString();
            killsCounter++;
            killsCounterTMP.text = (killsCounter < 10) ? ("0" + killsCounter.ToString()) : killsCounter.ToString();
        }
        else
        {
            // colocar en corrutina para pausa de 0.5 segundos
            victory = false;
            timeCounter.setTimeCounterIsOn(false);
            LoadScene("Finish");
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
                enemySmallSpeed = 6.0f;
                enemyBigSpeed = 3.0f;
                enemySpawnInterval = 20.0f;
                powerUpSpawnInterval = 5.0f;
                powerUpDuration = 15.0f;
                powerUpSlowDownDivider = 5.0f;
                break;
            case 1:     // hard
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

        // get components
        spheresCounterGO = GameObject.Find("SpheresCounter (TMP)");
        spheresCounterGO.GetComponent<TMP_Text>().text = spheresCounter.ToString();
        ghostCounterTMP = GameObject.Find("GhostCounter (TMP)").GetComponent<TMP_Text>();
        killsCounterTMP = GameObject.Find("KillsCounter (TMP)").GetComponent<TMP_Text>();
        timeCounter.setTimeCounterTMP(GameObject.Find("TimeCounter (TMP)").GetComponent<TMP_Text>());
        enemySpawnsGO = GameObject.Find("Enemy Spawns");
        powerUpSpawnsGO = GameObject.Find("Item Spawns");
        bunkerDoorGO = GameObject.Find("Door");
        bunkerRoofGO = GameObject.Find("Roof");

        // start time counter
        timeCounter.setTimeCounterIsOn(true);

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
            // update 'ghostCounter'
            ghostCounter = GameObject.FindGameObjectsWithTag("Enemy").Length;
            ghostCounterTMP.text = (ghostCounter < 10) ? ("0" + ghostCounter.ToString()) : ghostCounter.ToString();
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

    // after gathering all spheres, open bunker
    private IEnumerator bunkerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        spheresCounterGO.gameObject.SetActive(false);
        bunkerDoorGO.gameObject.SetActive(false);
        bunkerRoofGO.gameObject.SetActive(false);
    }

    // when crossing checker line, finish the game
    private IEnumerator checkerCoroutine()
    {
        victory = true;
        yield return new WaitForSeconds(0.5f);
        LoadScene("Finish");
        StartCoroutine ( finishCoroutine() );
    }
}

public partial class GameManager : MonoBehaviour
{
    // didn't know how to get these programatically
    public Mesh mesh001;
    public Mesh mesh002;

    // components
    private TMP_Text gameoverTMP;
    private GameObject characterStatue;
    private TMP_Text timeTMP;
    private TMP_Text killsTMP;
    private Button button;
    // getters and setters

    // coroutines
    // start scene 'Finish'
    private IEnumerator finishCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        gameoverTMP = GameObject.Find("Text (TMP) (0)").GetComponent<TMP_Text>();
        gameoverTMP.text = (victory) ? "Has Ganado" : "Has perdido";
        characterStatue = GameObject.Find("CharacterStatue");
        characterStatue.GetComponent<MeshFilter>().mesh = (victory) ? mesh001 : mesh002;
        timeTMP = GameObject.Find("Text (TMP) (2)").GetComponent<TMP_Text>();
        timeTMP.text = timeCounter.getTimeCounterText();
        killsTMP = GameObject.Find("Text (TMP) (4)").GetComponent<TMP_Text>();
        killsTMP.text = killsCounter.ToString();
        button = GameObject.Find("Button (0)").GetComponent<Button>();
        button.onClick.AddListener(() => LoadScene("Menu"));
        StopAllCoroutines();
    }

    // start scene 'Credits'
    private IEnumerator creditsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        button = GameObject.Find("Button (0)").GetComponent<Button>();
        button.onClick.AddListener(() => LoadScene("Menu"));
        StopAllCoroutines();
    }
}