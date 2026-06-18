using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPoint1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("ÉSÅ[ÉãÇµÇ‹ÇµÇΩÅI");

            SceneManager.LoadScene("GameClearScene1");
        }
    }
}