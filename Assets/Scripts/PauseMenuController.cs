using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{

    public GameObject pauseMenuCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuCanvas.activeSelf)
            {
                pauseMenuCanvas.SetActive(false);
                Time.timeScale = 1f; // Resume the game
            }
            else
            {
                pauseMenuCanvas.SetActive(true);
                Time.timeScale = 0f; // Pause the game
            }
        }
    }


    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
