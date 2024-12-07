using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;

public class NetManager : ManagerBase
{
    public static NetManager Instance = null;

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

    private void Start()
    {
        Connected();
    }

    private ClientPeer client = new ClientPeer("10.25.55.88", 6666);

    public void Connected() 
    {
        client.Connect();
    }

    private void Update()
    {
        if (client == null) 
        {
            return;
        }

        while (client.socketMsgQueue.Count > 0) {
            SocketMsg msg = client.socketMsgQueue.Dequeue();
            //TODO 操作这个msg
        }
    }
}
