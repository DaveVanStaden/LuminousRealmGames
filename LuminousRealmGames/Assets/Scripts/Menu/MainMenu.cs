using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the first level
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        // Quit the game
        Application.Quit();
    }
}
