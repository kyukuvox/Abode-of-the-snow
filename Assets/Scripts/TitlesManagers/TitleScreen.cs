using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Charge ta scène de jeu
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
