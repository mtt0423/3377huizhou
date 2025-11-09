using UnityEngine;
using UnityEngine.InputSystem;

public class BoatForwardTurnController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float maxForwardSpeed = 5f;
    public float maxReverseSpeed = 2f;
    public float acceleration = 8f;
    public float deceleration = 10f;

    [Header("转向设置")]
    public float turnSpeed = 2f;
    public float turnAcceleration = 5f;
    public float maxTurnSpeed = 3f;

    [Header("物理设置")]
    public float linearDrag = 1f;
    public float angularDrag = 2f;

    // 2D物理组件
    private Rigidbody2D rb2D;

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
        rb2D.gravityScale = 0f;
        rb2D.drag = linearDrag;
        rb2D.angularDrag = angularDrag;

        // 确保有2D碰撞体
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        Debug.Log("船舶控制已初始化：W/S前进后退，A/D转向");
    }

    void Update()
    {
        // 移除所有旧的输入检查代码
        // 只保留调试信息

        // 调试信息
        if (Keyboard.current.pKey.wasPressedThisFrame) // 使用新输入系统
        {
            Debug.Log($"速度: {currentSpeed:F2}, 转向: {currentTurn:F2}, 船头方向: {transform.up}");
        }

        // 显示控制提示 - 使用新输入系统
        if (Keyboard.current.hKey.wasPressedThisFrame) // 使用新输入系统
        {
            Debug.Log("控制说明: W-前进, S-后退, A-左转, D-右转");
        }
    }

    void FixedUpdate()
    {
        HandleForwardMovement();
        HandleTurning();
        ApplyAdvancedPhysics();
    }

    // 输入回调 - 这是主要的输入获取方式
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"接收到输入: {moveInput}"); // 添加调试
    }

    void HandleForwardMovement()
    {
        float targetSpeed = 0f;

        // 前进/后退输入
        if (moveInput.y > 0.1f) // W键 - 前进
        {
            targetSpeed = moveInput.y * maxForwardSpeed;
        }
        else if (moveInput.y < -0.1f) // S键 - 后退
        {
            targetSpeed = moveInput.y * maxReverseSpeed;
        }

        // 平滑加速/减速
        if (Mathf.Abs(targetSpeed) > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 应用前进/后退力（沿船头方向）
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            Vector2 moveForce = transform.up * currentSpeed;
            rb2D.AddForce(moveForce, ForceMode2D.Force);
        }
    }

    void HandleTurning()
    {
        float targetTurn = moveInput.x * maxTurnSpeed;

        // 平滑转向
        if (Mathf.Abs(targetTurn) > 0.1f)
        {
            currentTurn = Mathf.Lerp(currentTurn, targetTurn, turnAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentTurn = Mathf.Lerp(currentTurn, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 应用转向扭矩
        if (Mathf.Abs(currentTurn) > 0.1f)
        {
            rb2D.AddTorque(currentTurn, ForceMode2D.Force);
        }
    }

    void ApplyAdvancedPhysics()
    {
        // 防止侧向滑动 - 增加横向阻力
        Vector2 lateralVelocity = GetLateralVelocity();
        Vector2 counterForce = -lateralVelocity * 2f;
        rb2D.AddForce(counterForce, ForceMode2D.Force);

        // 速度限制
        Vector2 forwardVelocity = GetForwardVelocity();
        float currentForwardSpeed = forwardVelocity.magnitude;
        float maxSpeed = currentSpeed > 0 ? maxForwardSpeed : maxReverseSpeed;

        if (currentForwardSpeed > maxSpeed)
        {
            Vector2 limitedVelocity = forwardVelocity.normalized * maxSpeed;
            rb2D.velocity = new Vector2(limitedVelocity.x, limitedVelocity.y) + lateralVelocity * 0.5f;
        }
    }

    Vector2 GetForwardVelocity()
    {
        return Vector2.Dot(rb2D.velocity, transform.up) * transform.up;
    }

    Vector2 GetLateralVelocity()
    {
        return Vector2.Dot(rb2D.velocity, transform.right) * transform.right;
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 船头方向
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up * 2f);

        // 当前速度
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, rb2D.velocity);

        // 转向指示
        Gizmos.color = Color.red;
        Vector3 turnIndicator = transform.right * currentTurn;
        Gizmos.DrawRay(transform.position + Vector3.back * 0.5f, turnIndicator);
    }
}