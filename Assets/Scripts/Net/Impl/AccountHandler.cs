using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class AccountHandler : HandlerBase
{
    public override void OnReceiver(int subCode, object value)
    {
        switch (subCode)
        {
            case AccountCode.LOGIN:
                loginResponse((int)value);
                break;
            case AccountCode.REGIST_SRES:
                registResponse((int)value);
                break;
            default:
                break;
        }
    }
    private PromptMsg promptMsg=new PromptMsg();

    /// <summary>
    /// 登陆响应
    /// </summary>
    private void loginResponse(int result) {

        switch (result)
        {
            case 0:
                LoadSceneMsg msg = new LoadSceneMsg(1,() => {
                    Debug.Log("加载完成！");
                });
                Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE,msg);
                break;
            case -1:
                promptMsg.Change("账号不存在", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            case -2:
                promptMsg.Change("账号在线", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            case -3:
                promptMsg.Change("账号密码不匹配", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            default:
                break;
        }

        //if (value == "登陆成功")
        //{
        //    promptMsg.Change(value.ToString(), Color.green);
        //    Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
        //    return;
        //}

        ////登录错误
        //promptMsg.Change(value.ToString(), Color.red);
        //Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);

    }

    /// <summary>
    /// 注册相应
    /// </summary>
    /// <param name="value"></param>
    private void registResponse(int result) {

        switch (result)
        {
            case 0:
                promptMsg.Change("注册成功", Color.green);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            case -1:
                promptMsg.Change("账号已经存在", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            case -2:
                promptMsg.Change("账号输入不合法", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            case -3:
                promptMsg.Change("密码不合法", Color.red);
                Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
                break;
            default:
                break;
        }
        //if (value == "注册成功")
        //{
        //    promptMsg.Change(value.ToString(), Color.green);
        //    Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
        //    return;
        //}

        ////登录错误
        //promptMsg.Change(value.ToString(), Color.red);
        //Dispatch(AreaCode.UI, UIEvent.PROMT_MSG, promptMsg);
    }
}

