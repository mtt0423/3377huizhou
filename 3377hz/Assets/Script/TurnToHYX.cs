using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnToHYX : MonoBehaviour
{

    public Button HYXButton;
    public Button HYXPSBut1;
    public Button HYXPSBut2;
    public Button HYXPSBut3;

    public Button backBut;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnToHYXScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HYX0");
    }

    public void TurnToHYXPSScene1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS1");
    }

    public void TurnToHYXPSScene2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS2");
    }

    public void TurnToHYXPSScene3()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HYXPS3");
    }

    public void BackMaue()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("FigureSelect");
    }
}
