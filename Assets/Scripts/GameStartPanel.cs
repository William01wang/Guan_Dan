using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartPanel : UIBase
{
    private Button btnStart;
    private Button btnExit;
    public bool gameStartClicked=false;

    // Use this for initialization
    void Start()
    {
        btnStart = transform.Find("btnStart").GetComponent<Button>();
        btnExit = transform.Find("btnExit").GetComponent<Button>();

        btnStart.onClick.AddListener(StartClick);
        btnExit.onClick.AddListener(ExitClick);

    }

    public override void OnDestroy()
    {
        btnStart.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
    }

    private void StartClick()
    {
        gameStartClicked = true;
        gameObject.SetActive(false);
    }

    private void ExitClick()
    {
        SceneManager.LoadScene("Login");
    }

}
