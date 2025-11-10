using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

public class CloudTransition : MonoBehaviour
{
    public GameObject eventObj;
    public Button btnmain;
    public Animator animator;
    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        GameObject.DontDestroyOnLoad(this.eventObj);

        btnmain.onClick.AddListener(LoadSceneMain);
    }
    private void LoadSceneMain()
    {
        StartCoroutine(LoadScene(2));
    }
    IEnumerator LoadScene(int index)
    {
        animator.SetBool("fadein", false);
        animator.SetBool("fadeout", true);
        yield return new WaitForSeconds(1);

        AsyncOperation async =  SceneManager.LoadSceneAsync(index);
        async.completed += OnLoadScene;
    }

    private void OnLoadScene(AsyncOperation obj)
    {
        animator.SetBool("fadein", true);
        animator.SetBool("fadeout", false);
    }
}
