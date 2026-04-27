using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseContainer;
    public GameObject settingsContainer;

    [Header("State")]
    private bool isPaused = false;

    void Start()
    {
        pauseContainer.SetActive(false);
        settingsContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void OpenSettings()
    {
        pauseContainer.SetActive(false);
        settingsContainer.SetActive(true);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseContainer.SetActive(true);
    }


    public void CloseSettings()
    {
        settingsContainer.SetActive(false);
        pauseContainer.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseContainer.SetActive(false);
        settingsContainer.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
        {
            Application.Quit();
    }

}
