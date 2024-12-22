using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndPanel : UIBase
{
    private Button btnRestart;
    private Button btnExit;
    public Text txtTitle;
    public bool gameRestartClicked=false;

    // Use this for initialization
    void Start()
    {
        btnRestart = transform.Find("btnRestart").GetComponent<Button>();
        btnExit = transform.Find("btnExit").GetComponent<Button>();
        txtTitle = transform.Find("txtTitle").GetComponent<Text>();

        btnRestart.onClick.AddListener(RestartClick);
        btnExit.onClick.AddListener(ExitClick);

    }

    public override void OnDestroy()
    {
        btnRestart.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
    }

    private void RestartClick()
    {
        gameRestartClicked = true;
        gameObject.SetActive(false);
    }

    private void ExitClick()
    {
        SceneManager.LoadScene("Login");
    }

}
