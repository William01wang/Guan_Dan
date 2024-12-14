using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : UIBase
{
    private Button btnPass;
    public Text Rank;
    public bool btnPassClicked=false;

    // Use this for initialization
    void Start()
    {
        btnPass = transform.Find("btnPass").GetComponent<Button>();
        Rank = transform.Find("Rank").GetComponent<Text>();
        btnPass.onClick.AddListener(PassClick);
    }

    public override void OnDestroy()
    {
        btnPass.onClick.RemoveAllListeners();
    }

    private void PassClick()
    {
        //Dispatch(AreaCode.UI, UIEvent.PLAYERUI_PANEL_ACTIVE, false);
        setPanelActive(false);
        btnPassClicked = true;
    }
    public new void setPanelActive(bool active)
    {
        gameObject.SetActive(active);
    }

}
