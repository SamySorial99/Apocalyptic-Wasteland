using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public GameManager gm;

    void Awake()
    {
        gm = GameManager.GetInstance();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        gm.StartLevel();
        SceneManager.LoadScene("Wasteland");
    }

    public void Ranking()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
