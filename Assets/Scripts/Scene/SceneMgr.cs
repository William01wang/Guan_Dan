using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneMgr : ManagerBase
{
    public static SceneMgr Instance = null;

    private void Awake()
    {
        Instance = this;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        Add(SceneEvent.LOAD_SCENE, this);
    }

    
    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {   
            case SceneEvent.LOAD_SCENE:
                LoadSceneMsg msg = message as LoadSceneMsg;
                loadScene(msg);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 用来控制场景的
    /// </summary>
    private SceneManager sceneManager;


    /// <summary>
    /// 临时变量
    /// </summary>
    public Action OnSceneLoaded = null;


    private void loadScene(LoadSceneMsg msg) {
        if (msg.SceneBuildIndex != -1) {
            SceneManager.LoadScene(msg.SceneBuildIndex);
        }
        if (msg.SceneBuildName != null) {
            SceneManager.LoadScene(msg.SceneBuildName);
        }
        if (msg.OnSceneLoaded != null) { 
           OnSceneLoaded = msg.OnSceneLoaded;
        }
    }

    /// <summary>
    /// 当场景加载完成的时候调用
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (OnSceneLoaded != null) {
            OnSceneLoaded();
        }
    }

}


