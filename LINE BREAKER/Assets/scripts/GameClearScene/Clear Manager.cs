using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替え
using UnityEngine.EventSystems;

public class ClearManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    private Vector3 lastMousePosition;

    // クリア画面の最初に選択されるボタン（Retryボタンなど）を設定
    void Start()
    {
        if (firstSelectedButton != null)
        {
            // 最初に選択されるボタンを設定
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        // マウスの初期位置を保存
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (Vector3.Distance(Input.mousePosition, lastMousePosition) > 0.1f)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        lastMousePosition = Input.mousePosition;

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (verticalInput != 0 || horizontalInput != 0)
        {
            if (EventSystem.current.currentSelectedGameObject == null && firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }

    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 「タイトルへ」ボタンが押されたとき
    public void OnTitleButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); 
    }
}