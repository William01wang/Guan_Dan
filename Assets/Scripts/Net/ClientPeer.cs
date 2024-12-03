
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


/// <summary>
/// 客户端socket的封装
/// </summary>
public class ClientPeer 
{
    private Socket socket;

    private string ip;

    private int port;

    /// <summary>
    /// 构造连接对象
    /// </summary>
    /// <param name="ip">ip地址</param>
    /// <param name="port">端口号</param>
    public ClientPeer(string ip, int port) 
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ip = ip;
            this.port = port; 
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    public void Connect()
    {
        try
        {
            //连接服务器
            socket.Connect(ip, port);
            Debug.Log("连接服务器成功！");

            startReceive();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    #region 接收数据
    //接收的数据缓冲区
    private byte[] receiveBuffer =new byte[1024];

    /// <summary>
    /// 一旦接收到数据，就存到缓存区里面吗
    /// </summary>
    private List<byte> dataCache = new List<byte>();

    private bool isProcessReceive = false;

    public Queue<SocketMsg> socketMsgQueue =new Queue<SocketMsg>();

    /// <summary>
    /// 开始异步接受数据
    /// </summary>
    private void startReceive() 
    {
        if (socket == null && socket.Connected) {
            Debug.Log("没有连接成功，无法发送数据");
            return;
        }


        socket.BeginReceive(receiveBuffer,0,1024,SocketFlags.None,receiveCallBack,socket);
    }

    /// <summary>
    /// 收到消息的回调
    /// </summary>
    /// <param name="ar"></param>
    private void receiveCallBack(IAsyncResult ar) 
    {
        try
        {
            int length = socket.EndReceive(ar);
            byte[] tmpByteArray = new byte[length];
            Buffer.BlockCopy(receiveBuffer, 0, tmpByteArray, 0, length);
            //处理收到的数据
            dataCache.AddRange(tmpByteArray);
            if (isProcessReceive == false) {
                processReceive();
            }
        }
        catch (Exception e) 
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    /// <summary>
    /// 处理收到的数据
    /// </summary>
    private void processReceive() 
    {
        isProcessReceive = true;

        //解析数据包
        byte[] data = EncodeTool.DecodePacket(ref dataCache);

        if (data == null)
        {
            isProcessReceive=false;
            return;
        }

        SocketMsg msg = EncodeTool.DecodeMsg(data);
        //存储消息等待处理
        socketMsgQueue.Enqueue(msg);

        //尾递归
        processReceive();
    }
    #endregion

    #region 发送数据

    public void Send(int opCode, int subCode, object value) 
    {
        SocketMsg msg = new SocketMsg(opCode,subCode,value);
        Send(msg);
    }

    public void Send(SocketMsg msg) {
        byte[] data = EncodeTool.EncodeMsg(msg);
        byte[] packet = EncodeTool.EncodePacket(data);

        try
        {
            socket.Send(packet);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }
    #endregion
}
