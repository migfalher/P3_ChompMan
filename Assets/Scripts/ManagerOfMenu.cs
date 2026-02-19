using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerOfMenu : MonoBehaviour
{
    // components
    private Button playButton;
    private Button creditsButton;
    private Button quitButton;
    private Button easyButton;
    private Button hardButton;
    private GameObject dificultyPanel;

    // variables

    // Awake is called once after the scripts gets initialized (I guess...)
    private void Awake()
    {
        playButton = GameObject.Find("Play Button").GetComponent<Button>();
        creditsButton = GameObject.Find("Credits Button").GetComponent<Button>();
        quitButton = GameObject.Find("Quit Button").GetComponent<Button>();
        easyButton = GameObject.Find("Easy Button").GetComponent<Button>();
        hardButton = GameObject.Find("Hard Button").GetComponent<Button>();
        dificultyPanel = GameObject.Find("Dif Panel");
        dificultyPanel.SetActive(false);
        dificultyPanel.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playButton.onClick.AddListener(() => StartCoroutine( dificultyCoroutine() ));
        creditsButton.onClick.AddListener(() => SceneManager.LoadScene("Credits"));
        quitButton.onClick.AddListener(() => Application.Quit());
        easyButton.onClick.AddListener(() => GlobalVariables.setDificulty(1));
        hardButton.onClick.AddListener(() => GlobalVariables.setDificulty(2));
    }

    // coroutine set dificulty before start game
    private IEnumerator dificultyCoroutine()
    {
        dificultyPanel?.SetActive(true);
        yield return new WaitUntil(() => GlobalVariables.getDificulty() > 0);
        SceneManager.LoadScene("Game");
    }
}
