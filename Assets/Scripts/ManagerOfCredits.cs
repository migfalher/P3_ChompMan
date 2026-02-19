using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerOfCredits : MonoBehaviour
{
    // variables
    private Button menuButton;

    // Awake is called once after the scripts gets initialized (I guess...)
    private void Awake()
    {
        menuButton = GameObject.Find("Button (0)").GetComponent<Button>();
        menuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }
}
