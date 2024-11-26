using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuFirst;
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }
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
