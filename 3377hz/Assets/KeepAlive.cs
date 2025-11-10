using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    void Awake()
    {
        // 强制顶层（永远不嵌套）
        transform.parent = null;
        // 防重复（只留1个）
        if (FindObjectsOfType<KeepAlive>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}