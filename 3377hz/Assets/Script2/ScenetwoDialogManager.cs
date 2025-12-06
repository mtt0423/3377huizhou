using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ========== 独立命名空间：彻底隔离类，避免冲突 ==========
namespace HuXueYanDialogTwo
{
    // 仅保留背景配置（删除物品/选项相关配置）
    [System.Serializable]
    public class BackgroundConfig
    {
        public string backgroundName;
        public Sprite backgroundSprite;
    }

    // 数据结构：适配胡雪岩CSV 7列格式
    public class DialogData
    {
        public string type;       // CSV第1列：标志（#/END1）
        public int id;            // CSV第2列：ID
        public string speakerName;// CSV第3列：人物
        public string position;   // CSV第4列：位置（左/右）
        public string content;    // CSV第5列：对话内容
        public string nextId;     // CSV第6列：跳转ID
        public string extProp;    // CSV第7列：扩展属性（仅背景）
    }
}

// ==================== 核心脚本（类名：ScenetwoDialogManager）====================
public class ScenetwoDialogManager : MonoBehaviour
{
    // ==== 基础配置（拖拽赋值）====
    public TextAsset dialogDataFile;       // 胡雪岩.CSV文件
    public TMP_Text dialogContentText;     // 对话文本框
    public TMP_Text nameText;              // 人物名字文本框
    public Image backgroundImage;          // 背景图组件
    public GameObject nextButton;          // 继续按钮
    public Image leftCharacterImage;       // 左侧人物立绘
    public Image rightCharacterImage;      // 右侧人物立绘

    // ==== 背景配置（仅保留必要项，命名空间限定）====
    public List<HuXueYanDialogTwo.BackgroundConfig> backgroundConfigs = new List<HuXueYanDialogTwo.BackgroundConfig>();

    // ==== 内部变量（适配胡雪岩CSV，避免冲突）====
    private List<HuXueYanDialogTwo.DialogData> dialogList = new List<HuXueYanDialogTwo.DialogData>();
    private int currentDialogID = 0;        // 胡雪岩CSV初始ID（从0开始）
    private string mainMenuSceneName = "MainMenu"; // 主界面场景名（需与Build Settings一致）

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 跨场景保留对话管理器
        ReadCSV(); // 读取胡雪岩CSV数据
    }

    void Start()
    {
        if (nameText != null) nameText.enabled = false; // 初始隐藏名字
        ShowCurrentDialog(currentDialogID); // 启动时显示初始剧情
    }

    // ==================== CSV读取（适配胡雪岩格式，无冗余逻辑）====================
    private void ReadCSV()
    {
        dialogList.Clear();
        if (dialogDataFile == null)
        {
            Debug.LogError("❌ 未赋值胡雪岩CSV文件！请拖拽CSV到dialogDataFile字段");
            return;
        }

        StringReader reader = new StringReader(dialogDataFile.text);
        int lineIndex = 0;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            lineIndex++;
            if (lineIndex == 1) continue; // 跳过CSV表头
            if (string.IsNullOrEmpty(line)) continue; // 跳过空行

            string[] columns = line.Split(',');
            if (columns.Length < 7)
            {
                Debug.LogWarning($"⚠️ CSV第{lineIndex}行列数不足，跳过：{line}");
                continue;
            }

            // 解析CSV数据（命名空间限定类，避免冲突）
            HuXueYanDialogTwo.DialogData data = new HuXueYanDialogTwo.DialogData
            {
                type = columns[0].Trim(),
                id = int.TryParse(columns[1].Trim(), out int id) ? id : -1,
                speakerName = columns[2].Trim(),
                position = columns[3].Trim(),
                content = columns[4].Trim(),
                nextId = columns[5].Trim(),
                extProp = columns[6].Trim()
            };

            if (data.id != -1)
            {
                dialogList.Add(data);
                Debug.Log($"✅ 读取胡雪岩剧情：ID={data.id}，人物={data.speakerName}");
            }
        }
        Debug.Log($"✅ 胡雪岩CSV读取完成，共{dialogList.Count}条剧情");
    }

    // ==================== 显示当前剧情（删除分支/物品逻辑）====================
    public void ShowCurrentDialog(int dialogId)
    {
        Debug.Log($"🎬 显示剧情 ID: {dialogId}");
        // 命名空间限定，避免类冲突
        HuXueYanDialogTwo.DialogData currentDialog = dialogList.Find(d => d.id == dialogId);
        if (currentDialog == null)
        {
            dialogContentText.text = "未找到对应剧情";
            Debug.LogError($"❌ 未找到ID为{dialogId}的剧情（检查胡雪岩CSV）");
            return;
        }

        currentDialogID = dialogId;
        HideAllInteractiveElements(); // 隐藏所有交互元素（仅继续按钮）
        UpdateBackground(currentDialog.extProp); // 切换背景

        // 仅处理“对话”和“结束”两种核心类型
        switch (currentDialog.type)
        {
            case "#": // 对话类型（胡雪岩CSV主要类型）
                ShowDialogContent(currentDialog);
                ShowName(currentDialog);
                ShowCharacters(currentDialog);
                nextButton.SetActive(true); // 显示继续按钮
                break;

            case "END1": // 胡雪岩CSV结束标志（适配CSV最后一行）
                dialogContentText.text = "对话结束，即将返回主界面...";
                HideName();
                HideCharacters();
                nextButton.SetActive(false); // 隐藏继续按钮
                Invoke("BackToMainMenu", 2f); // 延迟2秒返回主界面
                break;

            default:
                dialogContentText.text = "未知剧情类型";
                HideName();
                HideCharacters();
                nextButton.SetActive(true);
                break;
        }
    }

    // ==================== 核心辅助方法（命名空间限定类）====================
    // 显示对话内容
    private void ShowDialogContent(HuXueYanDialogTwo.DialogData dialog)
    {
        if (dialogContentText != null)
        {
            dialogContentText.text = string.IsNullOrEmpty(dialog.content) ? "" : dialog.content;
        }
    }

    // 隐藏交互元素（仅处理继续按钮，无其他冗余元素）
    private void HideAllInteractiveElements()
    {
        if (nextButton != null) nextButton.SetActive(false);
    }

    // 更新背景（适配胡雪岩CSV的extProp字段，直接匹配背景名）
    private void UpdateBackground(string extProp)
    {
        if (string.IsNullOrEmpty(extProp) || backgroundImage == null) return;

        string backgroundName = extProp.Trim(); // CSV的extProp直接是背景名（无|分隔）
        HuXueYanDialogTwo.BackgroundConfig bgConfig = backgroundConfigs.Find(bg => bg.backgroundName.Trim() == backgroundName);
        if (bgConfig != null && bgConfig.backgroundSprite != null)
        {
            backgroundImage.sprite = bgConfig.backgroundSprite;
            Debug.Log($"🎨 切换背景：{backgroundName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到背景配置：{backgroundName}（检查BackgroundConfigs列表）");
        }
    }

    // 显示人物立绘（根据CSV的“位置”字段控制左右立绘）
    private void ShowCharacters(HuXueYanDialogTwo.DialogData dialog)
    {
        if (leftCharacterImage != null)
        {
            leftCharacterImage.enabled = dialog.position.Trim() == "左";
            leftCharacterImage.color = leftCharacterImage.enabled ? Color.white : new Color(1, 1, 1, 0);
        }

        if (rightCharacterImage != null)
        {
            rightCharacterImage.enabled = dialog.position.Trim() == "右";
            rightCharacterImage.color = rightCharacterImage.enabled ? Color.white : new Color(1, 1, 1, 0);
        }
    }

    // 隐藏人物立绘
    private void HideCharacters()
    {
        if (leftCharacterImage != null) leftCharacterImage.color = new Color(1, 1, 1, 0);
        if (rightCharacterImage != null) rightCharacterImage.color = new Color(1, 1, 1, 0);
    }

    // 显示说话人名字
    private void ShowName(HuXueYanDialogTwo.DialogData dialog)
    {
        if (nameText == null) return;

        if (!string.IsNullOrEmpty(dialog.speakerName))
        {
            nameText.text = dialog.speakerName;
            nameText.enabled = true;
        }
        else
        {
            HideName();
        }
    }

    // 隐藏说话人名字
    private void HideName()
    {
        if (nameText != null) nameText.enabled = false;
    }

    // ==================== 继续按钮点击事件（核心剧情跳转）====================
    public void OnNextButtonClick()
    {
        HuXueYanDialogTwo.DialogData currentDialog = dialogList.Find(d => d.id == currentDialogID);
        if (currentDialog == null)
        {
            Debug.LogError($"❌ 未找到当前剧情（ID：{currentDialogID}）");
            return;
        }

        if (int.TryParse(currentDialog.nextId, out int nextId))
        {
            // 检查目标剧情ID是否存在
            if (dialogList.Exists(d => d.id == nextId))
            {
                ShowCurrentDialog(nextId);
            }
            else
            {
                Debug.LogError($"❌ 未找到跳转目标剧情（ID：{nextId}，检查CSV的nextId字段）");
            }
        }
        else
        {
            Debug.LogError($"❌ 跳转ID格式错误（当前ID：{currentDialogID}，跳转值：{currentDialog.nextId}）");
        }
    }

   
}