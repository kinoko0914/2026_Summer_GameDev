using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager1 : MonoBehaviour
{
    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene1");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}