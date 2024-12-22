using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : UIBase
{
    private Button btnPass;
    private Button btnBGMPlaying;
    private Button btnBGMPause;
    public Text Rank;
    public Text TimerP1;
    public Text TimerP2;
    public Text TimerP3;
    public Text TimerP4;
    public bool btnPassClicked=false;
    public bool BGMPlaying = true;

    // Use this for initialization
    void Start()
    {
        btnPass = transform.Find("btnPass").GetComponent<Button>();
        btnBGMPlaying = transform.Find("btnBGMPlaying").GetComponent<Button>();
        btnBGMPause = transform.Find("btnBGMPause").GetComponent<Button>();
        Rank = transform.Find("Rank").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP1").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP2").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP3").GetComponent<Text>();
        TimerP1 = transform.Find("TimerP1").GetComponent<Text>();
        btnPass.onClick.AddListener(PassClick);
        btnBGMPlaying.onClick.AddListener(BGMPlayingClick);
        btnBGMPause.onClick.AddListener(BGMPauseClick);

    }

    public override void OnDestroy()
    {
        btnPass.onClick.RemoveAllListeners();
        btnBGMPlaying.onClick.RemoveAllListeners();
        btnBGMPause.onClick.RemoveAllListeners();
    }

    private void PassClick()
    {
        setBtnActive(false);
        btnPassClicked = true;
    }

    private void BGMPlayingClick()
    {
        setBGMActive(false);
    }
    private void BGMPauseClick()
    {
        setBGMActive(true);
    }

    public void setBGMActive(bool active)
    {
        transform.Find("btnBGMPlaying").gameObject.SetActive(active);
        transform.Find("btnBGMPause").gameObject.SetActive(!active);
        if (active)
        {
            GetComponent<AudioSource>().Play();
        }
        else 
        {
            GetComponent<AudioSource>().Pause();
        }
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
