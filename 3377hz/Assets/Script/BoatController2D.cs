using UnityEngine;
using UnityEngine.InputSystem;

public class BoatController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float acceleration = 8f;
    public float deceleration = 10f;

    [Header("转向设置")]
    public float turnSpeed = 3f;
    public float maxTurnSpeed = 2f;

    // 2D物理组件
    private Rigidbody2D rb2D;
    private PlayerInput playerInput;

    // 输入和状态
    private Vector2 moveInput;
    private float currentSpeed;
    private float currentTurn;

    void Start()
    {
        // 获取或添加2D刚体
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }

        // 配置2D物理
        rb2D.gravityScale = 0f; // 重要：在2D中禁用重力！
        rb2D.drag = 0.5f;
        rb2D.angularDrag = 1f;
        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY; // 锁定Y轴移动

        // 确保有2D碰撞体
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        // 读取输入
        if (playerInput != null && playerInput.currentActionMap != null)
        {
            var moveAction = playerInput.currentActionMap.FindAction("Move");
            if (moveAction != null)
            {
                moveInput = moveAction.ReadValue<Vector2>();
            }
        }

        // 调试
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"2D输入: {moveInput}, 速度: {currentSpeed:F2}");
        }
    }

    void FixedUpdate()
    {
        HandleMovement2D();
        HandleTurning2D();
    }

    // 输入回调
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void HandleMovement2D()
    {
        float targetSpeed = moveInput.y * moveSpeed;

        // 平滑加速/减速
        if (Mathf.Abs(targetSpeed) > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 应用2D移动力
        Vector2 moveForce = transform.right * currentSpeed; // 在2D中通常使用right作为前向
        rb2D.AddForce(moveForce, ForceMode2D.Force);
    }

    void HandleTurning2D()
    {
        float targetTurn = moveInput.x * maxTurnSpeed;

        // 平滑转向
        if (Mathf.Abs(targetTurn) > 0.1f)
        {
            currentTurn = Mathf.Lerp(currentTurn, targetTurn, turnSpeed * Time.fixedDeltaTime);
        }
        else
        {
            currentTurn = Mathf.Lerp(currentTurn, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 应用2D旋转
        if (Mathf.Abs(currentTurn) > 0.1f)
        {
            rb2D.AddTorque(currentTurn, ForceMode2D.Force);
        }
    }

    // 2D调试可视化
    void OnDrawGizmos()
    {
        // 移动方向
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * 2f);

        // 速度指示
        if (rb2D != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, rb2D.velocity);
        }
    }
}