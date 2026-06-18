using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替え
using UnityEngine.EventSystems;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    private Vector3 lastMousePosition;

    // タイトル画面の最初に選択されるボタンを設定
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
        // マウスの位置が前回の位置から動いた場合、選択を解除
        if (Vector3.Distance(Input.mousePosition, lastMousePosition) > 0.1f)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        lastMousePosition = Input.mousePosition;

        // 十字キーやスティック入力があった場合、最初のボタンを再度選択
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if(verticalInput != 0 || horizontalInput != 0)
        {
            if (EventSystem.current.currentSelectedGameObject == null && firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
    }

    private void UpdateSecection(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("GameScene"); // ゲームシーンに切り替え
    }

    public void OnStartButtonClick1()
    {
        SceneManager.LoadScene("GameScene1"); // ゲームシーンに切り替え
    }
}
