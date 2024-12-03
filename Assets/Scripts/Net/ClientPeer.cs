
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


/// <summary>
/// �ͻ���socket�ķ�װ
/// </summary>
public class ClientPeer 
{
    private Socket socket;

    private string ip;

    private int port;

    /// <summary>
    /// �������Ӷ���
    /// </summary>
    /// <param name="ip">ip��ַ</param>
    /// <param name="port">�˿ں�</param>
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
            //���ӷ�����
            socket.Connect(ip, port);
            Debug.Log("���ӷ������ɹ���");

            startReceive();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    #region ��������
    //���յ����ݻ�����
    private byte[] receiveBuffer =new byte[1024];

    /// <summary>
    /// һ�����յ����ݣ��ʹ浽������������
    /// </summary>
    private List<byte> dataCache = new List<byte>();

    private bool isProcessReceive = false;

    public Queue<SocketMsg> socketMsgQueue =new Queue<SocketMsg>();

    /// <summary>
    /// ��ʼ�첽��������
    /// </summary>
    private void startReceive() 
    {
        if (socket == null && socket.Connected) {
            Debug.Log("û�����ӳɹ����޷���������");
            return;
        }


        socket.BeginReceive(receiveBuffer,0,1024,SocketFlags.None,receiveCallBack,socket);
    }

    /// <summary>
    /// �յ���Ϣ�Ļص�
    /// </summary>
    /// <param name="ar"></param>
    private void receiveCallBack(IAsyncResult ar) 
    {
        try
        {
            int length = socket.EndReceive(ar);
            byte[] tmpByteArray = new byte[length];
            Buffer.BlockCopy(receiveBuffer, 0, tmpByteArray, 0, length);
            //�����յ�������
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
    /// �����յ�������
    /// </summary>
    private void processReceive() 
    {
        isProcessReceive = true;

        //�������ݰ�
        byte[] data = EncodeTool.DecodePacket(ref dataCache);

        if (data == null)
        {
            isProcessReceive=false;
            return;
        }

        SocketMsg msg = EncodeTool.DecodeMsg(data);
        //�洢��Ϣ�ȴ�����
        socketMsgQueue.Enqueue(msg);

        //β�ݹ�
        processReceive();
    }
    #endregion

    #region ��������

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
