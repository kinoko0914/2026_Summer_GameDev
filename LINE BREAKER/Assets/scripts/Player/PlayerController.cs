using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float jumpForce;

    [Header("落下死（ゲームオーバー）設定")]
    [SerializeField] private float deathYThreshold = -10f;

    [Header("接地判定の設定")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("ワイヤーアクション設定")]
    [SerializeField] private LayerMask canGrappleLayer;
    [SerializeField] private float swingForce = 1f;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    private SpringJoint2D springJoint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    private float originalDrag;
    private bool isDashJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        if (rb != null)
        {
            originalDrag = rb.linearDamping;
        }
    }

    void Update()
    {
        if (groundCheck != null)
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

            if (isGrounded && !wasGrounded)
            {
                isDashJumping = false;
            }
        }

        // 死亡判定
        if (transform.position.y < deathYThreshold)
        {
            DieAndChangeScene();
        }

        // 移動・スイング処理
        if (rb != null)
        {
            if (isGrappling)
            {
                rb.AddForce(new Vector2(horizontalInput * swingForce, 0f), ForceMode2D.Force);
            }
            else
            {
                bool keepDashSpeed = (isGrounded && Keyboard.current.leftShiftKey.isPressed) || (isDashJumping && horizontalInput != 0f);
                float currentMaxSpeed = keepDashSpeed ? dashSpeed : walkSpeed;

                if (horizontalInput == 0f)
                {
                    isDashJumping = false;
                }

                rb.linearVelocity = new Vector2(horizontalInput * currentMaxSpeed, rb.linearVelocity.y);
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGrapple();
        }
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

    private void DieAndChangeScene()
    {
        UnityEngine.Debug.Log("画面外に落下しました！ゲームオーバーシーンに移行します。");

        if (isGrappling)
        {
            StopGrapple();
        }

        SceneManager.LoadScene("GameOverScene");
    }

    private void StartGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePos - (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, canGrappleLayer);

        if (hit.collider != null)
        {
            UnityEngine.Debug.Log("ワイヤーがヒットしました！: " + hit.collider.name);
            grapplePoint = hit.point;

            springJoint = gameObject.AddComponent<SpringJoint2D>();
            springJoint.autoConfigureDistance = false;

            springJoint.distance = Vector2.Distance(transform.position, grapplePoint);
            springJoint.connectedAnchor = grapplePoint;

            springJoint.frequency = 0f;
            springJoint.dampingRatio = 0f;

            rb.linearDamping = 0f;
            isDashJumping = false;

            lineRenderer.positionCount = 2;
            isGrappling = true;
        }
    }

    private void StopGrapple()
    {
        if (isGrappling)
        {
            UnityEngine.Debug.Log("ワイヤーを解除しました。");
        }
        isGrappling = false;
        lineRenderer.positionCount = 0;

        if (rb != null)
        {
            rb.linearDamping = originalDrag;
        }

        if (springJoint != null)
        {
            Destroy(springJoint);
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
            else if (isGrounded)
            {
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    isDashJumping = true;
                }
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-100, deathYThreshold, 0), new Vector3(100, deathYThreshold, 0));
    }
}