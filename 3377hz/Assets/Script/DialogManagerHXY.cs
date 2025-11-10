using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TextAsset dialogDataFile;
    public SpriteRenderer spriteLeft;
    public SpriteRenderer spriteRight;

    public TMP_Text nameText;
    public TMP_Text dialogText;

    public List<Sprite> sprites = new List<Sprite>();
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    public int dialogIndex = 0;//当前对话索引值
    public string[] dialogRows;//对话文本,按行分割

    public Button nextButton; 

    public GameObject P1;
    public GameObject P2;
    public GameObject P2_1;
    public GameObject P3;
    public GameObject P4;
    public GameObject P5;
    public GameObject P6;
    public GameObject P7;
    public GameObject P8;
    public GameObject P9;
    public GameObject P10;
    public GameObject P11;
    public GameObject P12;
    public GameObject P13;
    public GameObject P14;
    public GameObject P15;
    public GameObject P16;


    private void Awake()
    {
        imageDic["我"] = sprites[0];
        imageDic["胡雪岩"] = sprites[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadText(dialogDataFile);
        ShowDialogRow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;      
    }

    public void UpdateImage(string _name, string _position)
    {
        if (_position == "左")
        {
            spriteLeft.sprite = imageDic[_name];
        }
        else if (_position == "右")
        {
            spriteRight.sprite = imageDic[_name];
        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows =_textAsset.text.Split('\n');
        Debug.Log("读取成功");
    }

    public void ShowDialogRow()
    {
        foreach(var row in dialogRows)
        {
            string[] cells = row.Split(',');
            if (cells[0]=="#"&&int.Parse(cells[1]) == dialogIndex)
            {
                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if(cells[0] == "END1" && int.Parse(cells[1]) == dialogIndex)
            {
               // UpdateText("", "对话结束");
               // nextButton.gameObject.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPeriodSelect");

                break;
            }
            else if(cells[0] == "NEXT1" && int.Parse(cells[1]) == dialogIndex)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS1");
                
                break;
            }
            else if (cells[0] == "NEXT2" && int.Parse(cells[1]) == dialogIndex)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS2");
                break;
            }
            else if (cells[0] == "NEXT3" && int.Parse(cells[1]) == dialogIndex)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS3");
                break;
            }
            else if( cells[0] =="P1" && int.Parse(cells[1]) == dialogIndex)
            {
                P1.SetActive(true);
                P2_1.SetActive(true);
                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P2" && int.Parse(cells[1]) == dialogIndex)
            {
                P2.SetActive(true);
                P1.SetActive(false);
                P2_1.SetActive(false);
                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P3" && int.Parse(cells[1]) == dialogIndex)
            {
                P3.SetActive(true);
                
                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P4" && int.Parse(cells[1]) == dialogIndex)
            {
                P4.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P5" && int.Parse(cells[1]) == dialogIndex)
            {
                P5.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P6" && int.Parse(cells[1]) == dialogIndex)
            {
                P6.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P7" && int.Parse(cells[1]) == dialogIndex)
            {
                P7.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P8" && int.Parse(cells[1]) == dialogIndex)
            {
                P8.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P9" && int.Parse(cells[1]) == dialogIndex)
            {
                P9.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P10" && int.Parse(cells[1]) == dialogIndex)
            {
                P10.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P11" && int.Parse(cells[1]) == dialogIndex)
            {
                P11.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P12" && int.Parse(cells[1]) == dialogIndex)
            {
                P12.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P13" && int.Parse(cells[1]) == dialogIndex)
            {
                P13.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P14" && int.Parse(cells[1]) == dialogIndex)
            {
                P14.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P15" && int.Parse(cells[1]) == dialogIndex)
            {
                P15.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "P16" && int.Parse(cells[1]) == dialogIndex)
            {
                P16.SetActive(true);

                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2], cells[4]);
                dialogIndex = int.Parse(cells[5]);
                break;
            }
        }
    }

    public void OnNextButtonClick()
    {
        ShowDialogRow();
    }

}
