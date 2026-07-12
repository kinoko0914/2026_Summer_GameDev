using UnityEngine;
using UnityEngine.InputSystem; 

public class AimStandardController : MonoBehaviour
{
    [Header("éQè∆")]
    [SerializeField] private PlayerController player; 
    [SerializeField] private Transform wireFirePoint; 

    [Header("ê›íË")]
    [SerializeField] private float standardDistance = 15f; 

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null) player = GetComponentInParent<PlayerController>();

        SetCrosshairVisible(false);
    }

    void Update()
    {
        if (player == null || spriteRenderer == null) return;

        Vector2 aimDirection = player.AimDirection;

        Vector2 startPos = wireFirePoint != null ? (Vector2)wireFirePoint.position : (Vector2)player.transform.position;

        if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.1f)
        {
            SetCrosshairVisible(true);

            Vector2 targetPos = startPos + aimDirection * standardDistance;
            transform.position = targetPos;
        }
        else
        {
            SetCrosshairVisible(false);
        }
    }

    private void SetCrosshairVisible(bool visible)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
    }
}