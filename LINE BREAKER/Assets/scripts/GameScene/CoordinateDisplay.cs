using UnityEngine;
using TMPro; 

public class CoordinateDisplay : MonoBehaviour
{
    [Header("表示するTextMeshProテキスト")]
    [SerializeField] private TextMeshProUGUI coordText;

    [Header("追跡するプレイヤーのTransform")]
    [SerializeField] private Transform playerTransform;

    void Update()
    {
        if (coordText != null && playerTransform != null)
        {
            Vector3 pos = playerTransform.position;

            coordText.text = $"Player: (X: {pos.x.ToString("F1")}, Y: {pos.y.ToString("F1")})";
        }
    }
}