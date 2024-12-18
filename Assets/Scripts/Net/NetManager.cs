using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class NetManager : ManagerBase
{
    public static NetManager Instance = null;

    


    private ClientPeer client= new ClientPeer("127.0.0.1", 6666);

    private void Start()
    {
        client.Connect();
    }

    private void Update()
    {
        if (client == null)
            return;

        while (client.SocketMsgQueue.Count > 0)
        {
            SocketMsg msg = client.SocketMsgQueue.Dequeue();
            //TOTO 操作这个msg
            procseeSocketMsg(msg);
        }
    }

    #region 处理接收到的服务器发来的消息

    HandlerBase accountHandler = new AccountHandler();

    /// <summary>
    /// 接收网络的消息
    /// </summary>
    private void procseeSocketMsg(SocketMsg msg) {
        switch(msg.OpCode)
        {
            case OpCode.ACCOUNT:
                accountHandler.OnReceiver(msg.SubCode, msg.Value);
                break;
            default:
                break;  
        }
    }

    #endregion

    #region 处理客户端内部给服务器发消息的事件

    private void Awake()
    {
        Instance = this;

        Add(0, this);

    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case 0:
                client.Send(message as SocketMsg);
                break;
            default:
                break;
        }
    }
    #endregion
}

