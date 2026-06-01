using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}