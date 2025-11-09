using UnityEngine;

public class BoatWakeEffect : MonoBehaviour
{
    [Header("波纹粒子系统")]
    public ParticleSystem wakeParticleSystem;
    public ParticleSystem bowWaveParticleSystem;

    [Header("波纹参数")]
    public float maxEmissionRate = 50f;
    public float minEmissionRate = 5f;
    public float wakeWidthMultiplier = 2f;
    public float wakeLengthMultiplier = 3f;

    [Header("速度相关效果")]
    public float minSpeedForWake = 0.5f;
    public float maxSpeedForWake = 5f;

    private Rigidbody2D rb2D;
    private BoatForwardTurnController2D boatController;
    private ParticleSystem.EmissionModule wakeEmission;
    private ParticleSystem.MainModule wakeMain;
    private ParticleSystem.ShapeModule wakeShape;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boatController = GetComponent<BoatForwardTurnController2D>();

        if (wakeParticleSystem != null)
        {
            wakeEmission = wakeParticleSystem.emission;
            wakeMain = wakeParticleSystem.main;
            wakeShape = wakeParticleSystem.shape;
        }

        Debug.Log("波纹效果系统已初始化");
    }

    void Update()
    {
        UpdateWakeEffects();
    }

    void UpdateWakeEffects()
    {
        if (wakeParticleSystem == null || rb2D == null) return;

        // 获取小船速度
        float speed = rb2D.velocity.magnitude;
        bool isMoving = speed > minSpeedForWake;

        // 控制波纹发射
        if (isMoving)
        {
            // 根据速度计算发射率
            float speedRatio = Mathf.InverseLerp(minSpeedForWake, maxSpeedForWake, speed);
            float emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, speedRatio);

            wakeEmission.rateOverTime = emissionRate;

            // 根据速度调整波纹形状
            UpdateWakeShape(speed, speedRatio);

            // 确保粒子系统运行
            if (!wakeParticleSystem.isPlaying)
                wakeParticleSystem.Play();
        }
        else
        {
            // 停止发射
            wakeEmission.rateOverTime = 0f;
            if (wakeParticleSystem.isPlaying)
                wakeParticleSystem.Stop();
        }

        // 更新船首波浪效果
        UpdateBowWaveEffect(speed, isMoving);
    }

    void UpdateWakeShape(float speed, float speedRatio)
    {
        // 船尾波纹形状随速度变化
        // 高速时：更长更窄的波纹
        // 低速时：更短更宽的波纹

        float wakeLength = Mathf.Lerp(1f, wakeLengthMultiplier, speedRatio);
        float wakeWidth = Mathf.Lerp(1.5f, 0.5f, speedRatio);

        wakeShape.radius = wakeWidth * 0.1f;

        // 调整粒子生命周期，高速时粒子存活更久
        wakeMain.startLifetime = Mathf.Lerp(1f, 3f, speedRatio);

        // 调整粒子大小
        wakeMain.startSize = Mathf.Lerp(0.1f, 0.3f, speedRatio);
    }

    void UpdateBowWaveEffect(float speed, bool isMoving)
    {
        if (bowWaveParticleSystem == null) return;

        var bowEmission = bowWaveParticleSystem.emission;

        if (isMoving && speed > minSpeedForWake * 1.5f)
        {
            float speedRatio = Mathf.InverseLerp(minSpeedForWake * 1.5f, maxSpeedForWake, speed);
            bowEmission.rateOverTime = Mathf.Lerp(2f, 15f, speedRatio);

            if (!bowWaveParticleSystem.isPlaying)
                bowWaveParticleSystem.Play();
        }
        else
        {
            bowEmission.rateOverTime = 0f;
            if (bowWaveParticleSystem.isPlaying)
                bowWaveParticleSystem.Stop();
        }
    }

    // 转向时的特殊波纹效果
    public void OnTurn(float turnAmount)
    {
        if (wakeParticleSystem == null || Mathf.Abs(turnAmount) < 0.1f) return;

        // 转向时在外侧产生更强的波纹
        // 可以通过创建额外的粒子系统或调整现有系统来实现
    }
}