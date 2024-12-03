using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 封装的客户端
    /// </summary>
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += SendArgs_Completed;
        }

        #region 接收数据

#pragma warning disable CS0436 // Type conflicts with imported type
        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg msg);
#pragma warning restore CS0436 // Type conflicts with imported type

        /// <summary>
        /// 一个消息解析完成的回调
        /// </summary>
        public ReceiveCompleted receiveCompleted;

        /// <summary>
        /// 一旦接受到数据，就存到缓存区里面
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        /// <summary>
        /// 接收的异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        /// <summary>
        /// 是否正在处理接收的数据
        /// </summary>
        private bool isReceiveProcess = false;

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            //防止这个方法被调用两次，添加了一个bool类型的变量来进行区分
            if (!isReceiveProcess)
            {
                processReceive();
            }
        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void processReceive()
        {
            isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref dataCache);

            //如果数据包没有解析成功
            if (data == null)
            {
                isReceiveProcess = false;
                return;
            }

            SocketMsg msg = EncodeTool.DecodeMsg(data);
            //回调给上层
            if (receiveCompleted != null)
            {
                receiveCompleted(this, msg);
            }
            //尾递归
            processReceive();
        }
        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnected()
        {

            try
            {
                //清空数据
                dataCache.Clear();
                isReceiveProcess = false;
                //TODO 给发送数据那里预留的
                sendQueue.Clear();
                isSendProcess = false;

                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
                ClientSocket = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送的消息的一个队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private bool isSendProcess = false;

        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs SendArgs;

        /// <summary>
        /// 发送的时候发现断开连接的回调
        /// </summary>
        /// <param name="clientPeer"></param>
        /// <param name="reason"></param>
        public delegate void SendDisconnect(ClientPeer clientPeer, string reason);

        public SendDisconnect sendDisconnect;

        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            //存入消息队列里面
            sendQueue.Enqueue(packet);
            if (!isSendProcess) { 
                send();
            }
        }

        private void send() {
            isSendProcess = true;

            //如果数据的条数等于0的话，就停止发送
            if (sendQueue.Count == 0) { 
                isSendProcess= false;
                return;
            }

            //取出一条数据
            byte[] packet =sendQueue.Dequeue();
            //设置消息发送异步套接字操作的发送数据缓冲区
            SendArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendPacketsAsync(SendArgs);
            if (result == false) {
               processSend();
            }
        }

        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e) {
            processSend();
        }

        /// <summary>
        /// 当异步发送请求完成的时候调用
        /// </summary>
        private void processSend() {
            //发送的有没有错误
            if (SendArgs.SocketError != SocketError.Success)
            {
                //发送出错了 客户端断开连接了
                sendDisconnect(this, SendArgs.SocketError.ToString());
            }
            else 
            {
                send();
            }
        }
        #endregion
    }
}
