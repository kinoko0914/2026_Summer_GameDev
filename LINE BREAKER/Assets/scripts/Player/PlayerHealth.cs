using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class PlayerHealth : MonoBehaviour
{
    [Header("最大ライフ（残機）")]
    [SerializeField] private int maxLife = 3;

    [Header("表示するTextMeshProテキスト")]
    [SerializeField] private TextMeshProUGUI lifeText;

    private int currentLife;

    void Start()
    {
        currentLife = maxLife;
        UpdateLifeUI();
    }

    public void TakeDamage()
    {
        currentLife--; 
        UpdateLifeUI(); 

        if (currentLife <= 0)
        {
            GameOver();
        }
    }

    void UpdateLifeUI()
    {
        if (lifeText != null)
        {
            lifeText.text = $"Life: {currentLife}";
        }
    }

    void GameOver()
    {
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            // 以前PlayerController内に作った死亡時処理を使い回すか、直接シーンを切り替えます
            SceneManager.LoadScene("GameOverScene");
        }
    }
}