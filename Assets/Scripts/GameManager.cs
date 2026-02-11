using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Global variables
    private int dificulty { get; set; }

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
                break;
            default:
                break;
        }
    }

    // coroutine 'startGame'
    private IEnumerator StartGame() { yield return null; }
}
