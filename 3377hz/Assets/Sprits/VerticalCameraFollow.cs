using UnityEngine;

public class VerticalCameraFollow : MonoBehaviour
{
    [Header("垂直移动设置")]
    public float startYPosition = 0f;      // 开始自动移动的Y坐标
    public float endYPosition = 20f;       // 停止自动移动的Y坐标
    public float scrollSpeed = 2f;         // 自动移动速度

    [Header("跟随目标")]
    public Transform target;               // 小船对象

    [Header("小船控制")]
    public MonoBehaviour boatController;   // 小船控制脚本（可选）

    [Header("移动控制")]
    public bool enableAutoScroll = true;   // 是否启用自动滚动
    public bool scrollUpwards = true;      // 是否向上滚动

    // 内部状态
    private bool isScrolling = false;
    private bool hasReachedEnd = false;
    private float initialX;
    private Camera cam;

    void Start()
    {
        initialX = transform.position.x;
        cam = GetComponent<Camera>();

        // 确保小船控制脚本引用正确
        if (boatController == null && target != null)
        {
            boatController = target.GetComponent<MonoBehaviour>();
            if (boatController != null)
            {
                Debug.Log("自动找到小船控制脚本: " + boatController.GetType().Name);
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 desiredPosition;

        // 检查是否应该开始自动滚动
        if (!isScrolling && enableAutoScroll)
        {
            if (scrollUpwards && targetPosition.y >= startYPosition)
            {
                isScrolling = true;
                // 禁用小船控制（如果有）
                DisableBoatControls();
            }
            else if (!scrollUpwards && targetPosition.y <= startYPosition)
            {
                isScrolling = true;
                // 禁用小船控制（如果有）
                DisableBoatControls();
            }
        }

        // 自动滚动阶段
        if (isScrolling && !hasReachedEnd)
        {
            // 计算新的Y位置
            float currentY = transform.position.y;
            float newY = currentY;

            if (scrollUpwards)
            {
                newY += scrollSpeed * Time.deltaTime;

                // 检查是否到达终点
                if (newY >= endYPosition)
                {
                    newY = endYPosition;
                    hasReachedEnd = true;
                }
            }
            else
            {
                newY -= scrollSpeed * Time.deltaTime;

                // 检查是否到达终点
                if (newY <= endYPosition)
                {
                    newY = endYPosition;
                    hasReachedEnd = true;
                }
            }

            desiredPosition = new Vector3(initialX, newY, transform.position.z);
        }
        else if (!isScrolling)
        {
            // 跟随阶段：相机跟随小船在垂直方向移动
            desiredPosition = new Vector3(initialX, targetPosition.y, transform.position.z);
        }
        else
        {
            // 已经到达终点，保持位置不变
            desiredPosition = transform.position;
        }

        transform.position = desiredPosition;
    }

    // 禁用小船控制（如果有）
    void DisableBoatControls()
    {
        if (boatController != null)
        {
            // 使用反射调用 SetControlsEnabled 方法
            var method = boatController.GetType().GetMethod("SetControlsEnabled");
            if (method != null)
            {
                method.Invoke(boatController, new object[] { false });
                Debug.Log("已禁用小船控制");
            }
        }
    }

    // 启用小船控制（如果有）
    void EnableBoatControls()
    {
        if (boatController != null)
        {
            // 使用反射调用 SetControlsEnabled 方法
            var method = boatController.GetType().GetMethod("SetControlsEnabled");
            if (method != null)
            {
                method.Invoke(boatController, new object[] { true });
                Debug.Log("已启用小船控制");
            }
        }
    }

    // 公共方法，可用于外部控制
    public void StartAutoScroll()
    {
        isScrolling = true;
        hasReachedEnd = false;
        // 禁用小船控制（如果有）
        DisableBoatControls();
    }

    public void StopAutoScroll()
    {
        hasReachedEnd = true;
        // 启用小船控制（如果有）
        EnableBoatControls();
    }

    public void ResetCamera()
    {
        isScrolling = false;
        hasReachedEnd = false;
        // 启用小船控制（如果有）
        EnableBoatControls();
    }

    public bool HasReachedEnd()
    {
        return hasReachedEnd;
    }

    // 在编辑器中可视化移动范围
    void OnDrawGizmosSelected()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // 绘制相机视野
        Gizmos.color = Color.yellow;
        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));

        // 绘制开始自动移动的线
        Gizmos.color = Color.red;
        Vector3 startLineStart = new Vector3(transform.position.x - width / 2, startYPosition, 0);
        Vector3 startLineEnd = new Vector3(transform.position.x + width / 2, startYPosition, 0);
        Gizmos.DrawLine(startLineStart, startLineEnd);

        // 绘制停止自动移动的线
        Gizmos.color = Color.green;
        Vector3 endLineStart = new Vector3(transform.position.x - width / 2, endYPosition, 0);
        Vector3 endLineEnd = new Vector3(transform.position.x + width / 2, endYPosition, 0);
        Gizmos.DrawLine(endLineStart, endLineEnd);

        // 绘制移动方向箭头
        Gizmos.color = Color.blue;
        Vector3 arrowBase = new Vector3(transform.position.x, (startYPosition + endYPosition) / 2, 0);
        Vector3 arrowTip;

        if (scrollUpwards)
        {
            arrowTip = arrowBase + Vector3.up * 2;
            Gizmos.DrawLine(arrowBase, arrowTip);
            Gizmos.DrawLine(arrowTip, arrowTip + Vector3.down * 0.5f + Vector3.left * 0.5f);
            Gizmos.DrawLine(arrowTip, arrowTip + Vector3.down * 0.5f + Vector3.right * 0.5f);
        }
        else
        {
            arrowTip = arrowBase + Vector3.down * 2;
            Gizmos.DrawLine(arrowBase, arrowTip);
            Gizmos.DrawLine(arrowTip, arrowTip + Vector3.up * 0.5f + Vector3.left * 0.5f);
            Gizmos.DrawLine(arrowTip, arrowTip + Vector3.up * 0.5f + Vector3.right * 0.5f);
        }
    }
}