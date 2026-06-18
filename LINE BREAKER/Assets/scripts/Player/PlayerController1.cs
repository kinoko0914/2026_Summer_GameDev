using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController1 : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float jumpForce = 12f;

    [Header("落下死（ゲームオーバー）設定")]
    [SerializeField] private float deathYThreshold = -10f;

    [Header("接地判定の設定")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    [Header("ワイヤーアクション設定")]
    [SerializeField] private LayerMask canGrappleLayer;
    [SerializeField] private float swingForce = 30f;

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

    // Updateは「入力の監視」と「見た目（LineRenderer）の更新」だけに専念させる
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

        // 直接マウス入力を監視
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGrapple();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            StopGrapple();
        }

        // ワイヤーの見た目の更新（描画はUpdateで行うのが最も滑らかになります）
        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    // ★【ここが超重要！】すべての物理移動処理をここに集約
    // PCの性能や軽さに影響されず、常に世界共通の同じ物理挙動になります
    void FixedUpdate()
    {
        if (rb == null) return;

        if (isGrappling)
        {
            // ワイヤー中のスイング加速（Time.fixedDeltaTimeをかけることでフレームレートの差を完全に無くします）
            rb.AddForce(new Vector2(horizontalInput * swingForce * 10f * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse);
        }
        else
        {
            // 通常移動およびダッシュジャンプの慣性維持
            bool keepDashSpeed = (isGrounded && Keyboard.current.leftShiftKey.isPressed) || (isDashJumping && horizontalInput != 0f);
            float currentMaxSpeed = keepDashSpeed ? dashSpeed : walkSpeed;

            if (horizontalInput == 0f)
            {
                isDashJumping = false;
            }

            rb.linearVelocity = new Vector2(horizontalInput * currentMaxSpeed, rb.linearVelocity.y);
        }
    }

    private void DieAndChangeScene()
    {
        if (isGrappling)
        {
            StopGrapple();
        }
        SceneManager.LoadScene("GameOverScene1");
    }

    private void StartGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePos - (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, canGrappleLayer);

        if (hit.collider != null)
        {
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
            isGrappling = false;
            lineRenderer.positionCount = 0;

            if (rb != null)
            {
                rb.linearDamping = originalDrag;

                if (Mathf.Abs(rb.linearVelocity.x) > 2f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x * 1.1f, rb.linearVelocity.y);
                }
            }

            if (springJoint != null)
            {
                Destroy(springJoint);
            }
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