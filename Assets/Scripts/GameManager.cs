using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    // Global variables
    private int dificulty { get; set; }
    private bool victory { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dificulty = 0;
        victory = false;
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
    private float powerUpDuration { get; set; }
    private float powerUpSlowDownDivider { get; set; }
    // components
    private GameObject spheresCounterGO { get; set; }
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
        StartCoroutine( powerUpCoroutine() );
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
                break;
            case 1:     // hard
                break;
            default:
                break;
        }

        // get components
        spheresCounterGO = GameObject.Find("SpheresCounter (TMP)");
        spheresCounterGO.GetComponent<TMP_Text>().text = spheresCounter.ToString();
        bunkerDoorGO = GameObject.Find("Door");
        bunkerRoofGO = GameObject.Find("Roof");

        yield return null;
    }

    // execute powerUp effect
    private IEnumerator powerUpCoroutine()
    {
        yield return null;
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