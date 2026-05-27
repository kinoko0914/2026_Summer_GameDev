using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("接地判定の設定")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("ワイヤーアクション設定")]
    [SerializeField] private LayerMask canGrappleLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGrapple();
        }
        // マウスが離されたらワイヤーを解除する
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    private void StartGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePos - (Vector2)transform.position;

        // 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, canGrappleLayer);

        if (hit.collider != null)
        {
            Debug.Log("ワイヤーがヒットしました！: " + hit.collider.name);
            grapplePoint = hit.point;

            distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.distance = Vector2.Distance(transform.position, grapplePoint);
            distanceJoint.connectedAnchor = grapplePoint;

            lineRenderer.positionCount = 2;
            isGrappling = true;
        }
        else
        {
            Debug.Log("クリックした方向にワイヤーが刺さる壁がありません。");
        }
    }

    private void StopGrapple()
    {
        if (isGrappling)
        {
            Debug.Log("ワイヤーを解除しました。");
        }
        isGrappling = false;
        lineRenderer.positionCount = 0;

        if (distanceJoint != null)
        {
            Destroy(distanceJoint);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>();
        horizontalInput = moveVector.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && rb != null)
        {
            if (isGrappling)
            {
                StopGrapple();
            }
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}