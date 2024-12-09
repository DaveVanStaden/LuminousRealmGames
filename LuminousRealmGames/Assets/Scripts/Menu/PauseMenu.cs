using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public bool isPaused = false;
    [SerializeField] private GameObject pauseMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    public void PauseGame()
    {
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }
    public void TogglePause()
    {
        if (isPaused)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
    public void OpenSettings()
    {
        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }
}
