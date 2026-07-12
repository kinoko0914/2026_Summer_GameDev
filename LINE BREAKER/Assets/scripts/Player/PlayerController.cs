using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
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
    [SerializeField] private Transform wireFirePoint;

    [Header("照準の設定")]
    [SerializeField] private Transform aimStandard; 
    [SerializeField] private float aimDistance = 2f; 

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    private SpringJoint2D springJoint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    private float originalDrag;
    private bool isDashJumping = false;

    private Animator animator;

    private enum InputMode { Mouse, Gamepad }
    private InputMode currentInputMode = InputMode.Mouse;
    private Vector2 gamepadAimDirection = Vector2.right;
    private bool isDashPressed = false;

    public Vector2 AimDirection { get; private set; }
    public bool IsUsingGamepadAim { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        animator = GetComponentInChildren<Animator>();

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
                if (SoundManager.Instance != null) SoundManager.Instance.PlayLandSE();
            }
        }

        if (transform.position.y < deathYThreshold)
        {
            DieAndChangeScene();
        }

        UpdateAimDirection();

        bool isGrappleJustPressed = false;
        bool isGrappleJustReleased = false;

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) isGrappleJustPressed = true;
            if (Mouse.current.leftButton.wasReleasedThisFrame) isGrappleJustReleased = true;
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightShoulder.wasPressedThisFrame) isGrappleJustPressed = true;
            if (Gamepad.current.rightShoulder.wasReleasedThisFrame) isGrappleJustReleased = true;
        }

        if (isGrappleJustPressed)
        {
            StartGrapple();
        }
        else if (isGrappleJustReleased)
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            lineRenderer.SetPosition(0, wireFirePoint != null ? wireFirePoint.position : transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);

            if (horizontalInput > 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (horizontalInput < -0.1f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (isGrappling)
        {
            rb.AddForce(new Vector2(horizontalInput * swingForce * 10f * Time.fixedDeltaTime, 0f), ForceMode2D.Impulse);
        }
        else
        {
            bool keepDashSpeed = (isGrounded && isDashPressed) || (isDashJumping && horizontalInput != 0f);
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
        SceneManager.LoadScene("GameOverScene");
    }

    private void UpdateAimDirection()
    {
        Vector2 startPos = wireFirePoint != null ? (Vector2)wireFirePoint.position : (Vector2)transform.position;

        // マウスとコントローラーの入力リアルタイム自動判定
        if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.1f)
        {
            currentInputMode = InputMode.Gamepad;
        }
        else if (Mouse.current != null && (Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f || Mouse.current.leftButton.isPressed))
        {
            currentInputMode = InputMode.Mouse;
        }

        if (currentInputMode == InputMode.Gamepad)
        {
            if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.1f)
            {
                gamepadAimDirection = Gamepad.current.rightStick.ReadValue().normalized;
            }
            AimDirection = gamepadAimDirection;
            IsUsingGamepadAim = true;
        }
        else
        {
            if (Mouse.current != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                AimDirection = (mousePos - startPos).normalized;
            }
            IsUsingGamepadAim = false;
        }

        if (aimStandard != null)
        {
            aimStandard.position = startPos + AimDirection * aimDistance;
        }
    }

    private void StartGrapple()
    {
        Vector2 startPos = wireFirePoint != null ? (Vector2)wireFirePoint.position : (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(startPos, AimDirection, 30f, canGrappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;

            springJoint = gameObject.AddComponent<SpringJoint2D>();
            springJoint.autoConfigureDistance = false;

            springJoint.distance = Vector2.Distance(startPos, grapplePoint);
            springJoint.connectedAnchor = grapplePoint;

            springJoint.frequency = 0f;
            springJoint.dampingRatio = 0f;

            rb.linearDamping = 0f;
            isDashJumping = false;

            lineRenderer.positionCount = 2;
            isGrappling = true;

            if (SoundManager.Instance != null) SoundManager.Instance.PlayGrappleSE();

            if (animator != null)
            {
                animator.SetBool("IsGrappling", true);
            }
        }
    }

    private void StopGrapple()
    {
        if (isGrappling)
        {
            isGrappling = false;
            lineRenderer.positionCount = 0;

            if (SoundManager.Instance != null) SoundManager.Instance.PlayReleaseSE();

            if (rb != null)
            {
                rb.linearDamping = originalDrag;

                if (Mathf.Abs(rb.linearVelocity.x) > 2f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x * 1.1f, rb.linearVelocity.y);
                }

                if (animator != null)
                {
                    animator.SetBool("IsGrappling", false);

                    if (!isGrounded)
                    {
                        animator.SetTrigger("JumpTrigger");
                    }
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
                if (isDashPressed)
                {
                    isDashJumping = true;
                }
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                if (SoundManager.Instance != null) SoundManager.Instance.PlayJumpSE();

                if (animator != null)
                {
                    animator.SetTrigger("JumpTrigger");
                }
            }
        }
    }

    public void OnSprint(InputValue value)
    {
        isDashPressed = value.isPressed;
    }

    public void OnGrapple(InputValue value) { }
    public void OnAim(InputValue value) { }
    public void OnMouseMove(InputValue value) { }

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