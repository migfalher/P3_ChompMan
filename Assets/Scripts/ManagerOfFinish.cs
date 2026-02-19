using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerOfFinish : MonoBehaviour
{
    public Mesh mesh001;
    public Mesh mesh002;

    // components
    private MeshFilter characterMF;
    private TMP_Text gameoverTMP;
    private TMP_Text timeTMP;
    private TMP_Text killsTMP;
    private Button menuButton;

    // Awake
    private void Awake()
    {
        characterMF = GameObject.Find("Character").GetComponent<MeshFilter>();
        gameoverTMP = GameObject.Find("GameOver (TMP)").GetComponent<TMP_Text>();
        timeTMP = GameObject.Find("TimeText (TMP)").GetComponent<TMP_Text>();
        killsTMP = GameObject.Find("KillsText (TMP)").GetComponent<TMP_Text>();
        menuButton = GameObject.Find("Menu Button").GetComponent<Button>();
        menuButton.onClick.AddListener(() => BackToMenu());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GlobalVariables.getVictory())
        {
            gameoverTMP.text = "Has ganado";
            characterMF.mesh = mesh001;
        }
        else
        {
            gameoverTMP.text = "Has perdido";
            characterMF.mesh = mesh002;
        }
        
        timeTMP.text = GlobalVariables.getTimeText();
        killsTMP.text = GlobalVariables.getKillsCounter().ToString();
    }

    private void BackToMenu()
    {
        GlobalVariables.reset();
        SceneManager.LoadScene("Menu");
    }
}
