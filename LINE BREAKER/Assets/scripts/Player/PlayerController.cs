using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //移動処理
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    //接地判定
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 地面に設置しているかどうか
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, LayerMask.GetMask("Ground"));
    }

    void FixedUpdate()
    {
        // 水平方向の移動
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        // 水平方向の入力を取得
        Vector2 moveVector = value.Get<Vector2>();
        horizontalInput = moveVector.x;
    }

    public void OnJump(InputValue value)
    {
        // ジャンプの入力を取得
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity =new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // Unityのエディタ画面に接地判定の枠線を表示する
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
