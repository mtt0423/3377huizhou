using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ==================== 配置类（全部保留）====================
[System.Serializable]
public class ItemConfig
{
    public string itemName;
    public Sprite itemSprite;
    public Vector2 screenPosition;
    public Vector2 itemSize = new Vector2(100, 100);
}

[System.Serializable]
public class OptionConfig
{
    public string optionText;
    public Vector2 screenPosition;
}

[System.Serializable]
public class BackgroundConfig
{
    public string backgroundName;
    public Sprite backgroundSprite;
}

// ==================== 数据结构（适配CSV列）====================
public class DialogData
{
    public string type;       // CSV第1列：标志（@/#/￥/&/END11）
    public int id;            // CSV第2列：ID
    public string speakerName;// CSV第3列：人物（名字列）
    public string position;   // CSV第4列：位置（人物左右）
    public string content;    // CSV第5列：内容（对话/选项文本）
    public string nextId;     // CSV第6列：跳转（目标ID）
    public string extProp;    // CSV第7列：扩展属性（背景|物品）
}

// ==================== 核心管理脚本（类名改为 ScenesthreeDialogManager）====================
public class ScenesthreeDialogManager : MonoBehaviour
{
    // ==== 基础配置（拖拽赋值）====
    public TextAsset dialogDataFile;       // CSV文件
    public TMP_Text dialogContentText;     // 对话文本
    public TMP_Text nameText;              // 名字文本（第三列“人物”）
    public Image backgroundImage;          // 背景图
    public GameObject nextButton;          // 继续按钮
    public Transform optionParent;         // 分支选项父物体
    public Transform backgroundItemCanvas; // 可点击物品父物体

    // ==== 人物立绘配置 ====
    public Image leftCharacterImage;       // 左侧人物立绘
    public Image rightCharacterImage;      // 右侧人物立绘

    // ==== 预制体配置 ====
    public GameObject clickableItemPrefab; // 可点击物品预制体
    public GameObject optionButtonPrefab;  // 分支选项按钮预制体

    // ==== 可视化配置（Inspector中设置）====
    public List<ItemConfig> itemConfigs = new List<ItemConfig>();
    public List<OptionConfig> optionConfigs = new List<OptionConfig>();
    public List<BackgroundConfig> backgroundConfigs = new List<BackgroundConfig>();

    // ==== 分支按钮统一配置（大小/字体/颜色全在这里调整）====
    [Header("按钮大小与间距")]
    public Vector2 optionButtonSize = new Vector2(250, 60); // 按钮宽/高
    public int optionSpacing = 70; // 按钮垂直间距

    [Header("按钮文本样式")]
    public float optionFontSize = 22f; // 字体大小
    public Color optionFontColor = Color.black; // 字体颜色
    public TMP_FontAsset optionFont; // 自定义字体（可选）

    [Header("按钮背景颜色")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f); // 正常状态颜色（默认浅灰）
    public Color highlightedColor = new Color(1f, 1f, 1f); // 高亮状态颜色（默认白色）
    public Color pressedColor = new Color(0.6f, 0.6f, 0.6f); // 按压状态颜色（默认深灰）
    public Color disabledColor = new Color(0.4f, 0.4f, 0.4f); // 禁用状态颜色（默认灰色）
    public float colorTransitionDuration = 0.1f; // 颜色过渡时长（平滑切换）

    // ==== 内部变量 ====
    private List<DialogData> dialogList = new List<DialogData>();
    private int currentDialogID = 111;     // 初始剧情ID
    private int triggerBranchDialogId = 122; // 触发分支的剧情ID（你的CSV中是122）

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ReadCSV();
    }

    void Start()
    {
        if (nameText != null)
        {
            nameText.text = "测试-人物名字";
            nameText.enabled = true;
        }
        ShowCurrentDialog(currentDialogID);
    }

    // ==================== CSV读取（完全适配你的7列格式）====================
    private void ReadCSV()
    {
        dialogList.Clear();
        if (dialogDataFile == null)
        {
            Debug.LogError("❌ 未赋值CSV文件！");
            return;
        }

        StringReader reader = new StringReader(dialogDataFile.text);
        int lineIndex = 0;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            lineIndex++;
            if (lineIndex == 1) continue; // 跳过表头
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');
            if (columns.Length < 7)
            {
                Debug.LogWarning($"⚠️ CSV第{lineIndex}行列数不足，跳过：{line}");
                continue;
            }

            DialogData data = new DialogData
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
                // 打印所有分支选项（方便调试）
                if (data.type == "&")
                {
                    Debug.Log($"✅ 读取分支选项：ID={data.id}，文本={data.content}，跳转={data.nextId}");
                }
            }
        }
        Debug.Log($"✅ CSV读取完成，共{dialogList.Count}条剧情，其中分支选项{dialogList.FindAll(d => d.type == "&").Count}个");
    }

    // ==================== 显示当前剧情（修正分支触发逻辑）====================
    public void ShowCurrentDialog(int dialogId)
    {
        Debug.Log($"🎬 显示剧情 ID: {dialogId}");

        DialogData currentDialog = dialogList.Find(d => d.id == dialogId);
        if (currentDialog == null)
        {
            dialogContentText.text = "未找到对应剧情";
            Debug.LogError($"❌ 未找到ID为{dialogId}的剧情");
            return;
        }

        currentDialogID = dialogId;
        HideAllInteractiveElements();

        // 根据剧情类型处理
        switch (currentDialog.type)
        {
            case "@": // 旁白
                ShowDialogContent(currentDialog);
                HideName();
                HideCharacters();
                nextButton.SetActive(true);
                break;

            case "#": // 对话
                ShowDialogContent(currentDialog);
                ShowName(currentDialog);
                ShowCharacters(currentDialog);
                // 关键：如果当前剧情是触发分支的ID（122），则生成分支选项，隐藏继续按钮
                if (currentDialog.id == triggerBranchDialogId)
                {
                    GenerateOptions(); // 生成所有分支选项
                    nextButton.SetActive(false); // 分支场景不需要继续按钮
                }
                else
                {
                    nextButton.SetActive(true);
                }
                break;

            case "￥": // 可点击物品
                ShowDialogContent(currentDialog);
                HideName();
                HideCharacters();
                string itemName = GetItemNameFromExtProp(currentDialog.extProp);
                if (!string.IsNullOrEmpty(itemName) && int.TryParse(currentDialog.nextId, out int itemNextId))
                {
                    CreateClickableItem(itemName, itemNextId);
                }
                break;

            case "&": // 分支选项（不直接显示）
                ShowDialogContent(currentDialog);
                HideName();
                HideCharacters();
                nextButton.SetActive(true);
                break;

            case "END11": // 剧情结束
                dialogContentText.text = "剧情结束！感谢游玩～";
                HideName();
                HideCharacters();
                break;

            default:
                dialogContentText.text = "未知剧情类型";
                HideName();
                HideCharacters();
                nextButton.SetActive(true);
                break;
        }

        UpdateBackground(currentDialog.extProp);
    }

    // ==================== 分支选项生成（支持颜色/大小/字体全自定义）====================
    private void GenerateOptions()
    {
        if (optionButtonPrefab == null || optionParent == null)
        {
            Debug.LogError("❌ 分支选项预制体或父物体未赋值！");
            return;
        }

        // 读取所有&类型分支选项
        List<DialogData> allOptions = dialogList.FindAll(d => d.type == "&");
        Debug.Log($"🔍 找到{allOptions.Count}个分支选项（直接读取所有&类型行）");

        // 遍历生成所有选项按钮
        for (int i = 0; i < allOptions.Count; i++)
        {
            DialogData option = allOptions[i];
            string optionText = option.content.Trim();

            if (string.IsNullOrEmpty(optionText))
            {
                Debug.LogWarning("⚠️ 跳过空文本分支选项");
                continue;
            }
            if (!int.TryParse(option.nextId, out int targetId))
            {
                Debug.LogWarning($"⚠️ 分支选项「{optionText}」跳转ID无效（{option.nextId}），跳过");
                continue;
            }

            // 生成按钮
            GameObject optionBtn = Instantiate(optionButtonPrefab, optionParent);
            optionBtn.name = "Option_" + optionText;

            // 1. 设置按钮文本样式
            TMP_Text btnText = optionBtn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                btnText.text = optionText;
                btnText.fontSize = optionFontSize;
                btnText.color = optionFontColor;
                btnText.alignment = TextAlignmentOptions.Center;
                if (optionFont != null) btnText.font = optionFont;
            }

            // 2. 设置按钮大小和位置
            RectTransform btnRect = optionBtn.GetComponent<RectTransform>();
            if (btnRect != null)
            {
                btnRect.anchorMin = new Vector2(0.5f, 0.5f);
                btnRect.anchorMax = new Vector2(0.5f, 0.5f);
                btnRect.pivot = new Vector2(0.5f, 0.5f);
                btnRect.sizeDelta = optionButtonSize; // 统一大小

                // 位置：优先配置，无则自动排列
                OptionConfig targetConfig = optionConfigs.Find(config => config.optionText.Trim() == optionText);
                if (targetConfig != null)
                {
                    btnRect.anchoredPosition = targetConfig.screenPosition;
                }
                else
                {
                    float yPosition = 100 - i * optionSpacing;
                    btnRect.anchoredPosition = new Vector2(0, yPosition);
                }
                btnRect.localScale = Vector3.one;
            }

            // 3. 设置按钮背景颜色（核心！统一修改按钮状态色）
            Button btn = optionBtn.GetComponent<Button>();
            if (btn != null)
            {
                // 创建颜色过渡器（保证颜色切换平滑）
                ColorBlock colorBlock = new ColorBlock();
                colorBlock.normalColor = normalColor; // 正常状态
                colorBlock.highlightedColor = highlightedColor; // 鼠标悬浮/选中
                colorBlock.pressedColor = pressedColor; // 点击按压
                colorBlock.disabledColor = disabledColor; // 禁用（当前用不到）
                colorBlock.colorMultiplier = 1f; // 颜色强度
                colorBlock.fadeDuration = colorTransitionDuration; // 过渡时长

                btn.colors = colorBlock;

                // 绑定点击事件
                int finalTargetId = targetId;
                btn.onClick.AddListener(() =>
                {
                    HideAllInteractiveElements();
                    ShowCurrentDialog(finalTargetId);
                });
            }
        }

        Debug.Log($"📋 分支选项生成完成：{allOptions.Count}个按钮（大小：{optionButtonSize.x}x{optionButtonSize.y}，字体大小：{optionFontSize}）");
    }

    // ==================== 辅助方法（全部保留）====================
    private void ShowDialogContent(DialogData dialog)
    {
        if (dialogContentText != null)
        {
            dialogContentText.text = string.IsNullOrEmpty(dialog.content) ? "" : dialog.content;
        }
    }

    private void HideAllInteractiveElements()
    {
        if (nextButton != null) nextButton.SetActive(false);

        if (optionParent != null)
        {
            foreach (Transform child in optionParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (backgroundItemCanvas != null)
        {
            foreach (Transform child in backgroundItemCanvas)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private string GetItemNameFromExtProp(string extProp)
    {
        if (string.IsNullOrEmpty(extProp)) return "";
        string[] props = extProp.Split('|');
        return props.Length > 1 ? props[1].Trim() : "";
    }

    private void UpdateBackground(string extProp)
    {
        if (string.IsNullOrEmpty(extProp) || backgroundImage == null) return;

        string[] props = extProp.Split('|');
        string backgroundName = props[0].Trim();
        if (string.IsNullOrEmpty(backgroundName)) return;

        BackgroundConfig bgConfig = backgroundConfigs.Find(bg => bg.backgroundName.Trim() == backgroundName);
        if (bgConfig != null && bgConfig.backgroundSprite != null)
        {
            backgroundImage.sprite = bgConfig.backgroundSprite;
            Debug.Log($"🎨 切换背景：{backgroundName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到背景配置：{backgroundName}");
        }
    }

    private void ShowCharacters(DialogData dialog)
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

    private void HideCharacters()
    {
        if (leftCharacterImage != null)
        {
            leftCharacterImage.color = new Color(1, 1, 1, 0);
        }

        if (rightCharacterImage != null)
        {
            rightCharacterImage.color = new Color(1, 1, 1, 0);
        }
    }

    private void ShowName(DialogData dialog)
    {
        if (nameText == null)
        {
            Debug.LogWarning("⚠️ 未赋值nameText组件");
            return;
        }

        if (!string.IsNullOrEmpty(dialog.speakerName))
        {
            nameText.text = dialog.speakerName;
            nameText.enabled = true;
            Debug.Log($"🗣️ 显示说话人：{dialog.speakerName}");
        }
        else
        {
            HideName();
        }
    }

    private void HideName()
    {
        if (nameText != null)
        {
            nameText.enabled = false;
        }
    }

    private void CreateClickableItem(string itemName, int nextId)
    {
        if (clickableItemPrefab == null || backgroundItemCanvas == null)
        {
            Debug.LogError("❌ 可点击物品预制体或父物体未赋值！");
            return;
        }

        ItemConfig targetConfig = itemConfigs.Find(config => config.itemName.Trim() == itemName);
        if (targetConfig == null)
        {
            Debug.LogError($"❌ 未找到物品「{itemName}」的配置");
            return;
        }

        GameObject itemObj = Instantiate(clickableItemPrefab, backgroundItemCanvas);
        itemObj.name = "Item_" + itemName;

        Image itemImage = itemObj.GetComponent<Image>();
        if (itemImage != null && targetConfig.itemSprite != null)
        {
            itemImage.sprite = targetConfig.itemSprite;
        }

        RectTransform itemRect = itemObj.GetComponent<RectTransform>();
        if (itemRect != null)
        {
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = targetConfig.screenPosition;
            itemRect.sizeDelta = targetConfig.itemSize;
            itemRect.localScale = Vector3.one;
        }

        TMP_Text itemText = itemObj.GetComponentInChildren<TMP_Text>();
        if (itemText != null)
        {
            itemText.text = itemName;
        }

        Button itemBtn = itemObj.GetComponent<Button>();
        if (itemBtn != null)
        {
            itemBtn.onClick.AddListener(() =>
            {
                Destroy(itemObj);
                ShowCurrentDialog(nextId);
            });
        }

        Debug.Log($"📦 生成可点击物品：{itemName}");
    }

    // ==================== 继续按钮点击事件 ====================
    public void OnNextButtonClick()
    {
        DialogData currentDialog = dialogList.Find(d => d.id == currentDialogID);
        if (currentDialog == null)
        {
            Debug.LogError($"❌ 未找到当前剧情（ID：{currentDialogID}）");
            return;
        }

        if (int.TryParse(currentDialog.nextId, out int nextId))
        {
            DialogData nextDialog = dialogList.Find(d => d.id == nextId);
            if (nextDialog != null)
            {
                ShowCurrentDialog(nextId);
            }
            else
            {
                Debug.LogError($"❌ 未找到跳转目标剧情（ID：{nextId}）");
            }
        }
        else
        {
            Debug.LogError($"❌ 跳转ID格式错误（当前ID：{currentDialogID}，跳转值：{currentDialog.nextId}）");
        }
    }
}