using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Global variables
    private int dificulty { get; set; }

    // Scene 'Game' variables
    private bool enemyIsVulnerable { get; set; }
    private float enemySmallSpeed { get; set; }
    private float enemyBigSpeed { get; set; }
    private float powerUpDuration { get; set; }
    private float powerUpSlowDownDivider {  get; set; }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // trigger interactions
    public void TouchSphere(GameObject sphere)
    {
        Destroy(sphere);
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

    // coroutine starters
    // powerUp
    public void startPowerUpCoroutine() { StartCoroutine (powerUpCoroutine()); }
    // bunker
    public void startBunkerCoroutine() { }

    // coroutine 'game'
    private IEnumerator gameCoroutine()
    {
        // set 'Game' variables
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

        yield return null;
    }

    // coroutine 'powerUp'
    private IEnumerator powerUpCoroutine()
    {
        yield return null;
    }

    // coroutine 'bunker'
    private IEnumerator bunkerCoroutine()
    {
        yield return null;
    }
}
