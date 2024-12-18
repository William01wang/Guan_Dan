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
    public Text TimerP1;
    public Text TimerP2;
    public Text TimerP3;
    public Text TimerP4;
    public bool btnPassClicked=false;

    // Use this for initialization
    void Start()
    {
        btnPass = transform.Find("btnPass").GetComponent<Button>();
        Rank = transform.Find("Rank").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP1").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP2").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP3").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP1").GetComponent<Text>();
        btnPass.onClick.AddListener(PassClick);
    }

    public override void OnDestroy()
    {
        btnPass.onClick.RemoveAllListeners();
    }

    private void PassClick()
    {
        //Dispatch(AreaCode.UI, UIEvent.PLAYERUI_PANEL_ACTIVE, false);
        setBtnActive(false);
        btnPassClicked = true;
    }
    public void setBtnActive(bool active)
    {
        transform.Find("btnPass").gameObject.SetActive(active);
    }

    public void setTimerP1Active(bool active)
    {
        transform.Find("TimerP1").gameObject.SetActive(active);
    }
    public void setTimerP2Active(bool active)
    {
        transform.Find("TimerP2").gameObject.SetActive(active);
    }
    public void setTimerP3Active(bool active)
    {
        transform.Find("TimerP3").gameObject.SetActive(active);
    }
    public void setTimerP4Active(bool active)
    {
        transform.Find("TimerP4").gameObject.SetActive(active);
    }

}
