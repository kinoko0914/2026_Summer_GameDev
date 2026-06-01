using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("非表示・表示を切り替えるポーズメニューUI")]
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

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

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f;           
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);  
        Time.timeScale = 1f;           
        isPaused = false;
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f;           
        SceneManager.LoadScene("TitleScene"); 
    }
}