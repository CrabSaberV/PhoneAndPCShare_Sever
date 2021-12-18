using MyServer.Message;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.NetWork
{
    /// <summary>
    /// 管理，连接的相关事务（创建线程，分发协议）
    /// 主要负责接收客户端的消息，解析并回调分发
    /// </summary>
    class NetworkManager
    {
        private static NetworkManager _Inst = new NetworkManager();

        public static NetworkManager Inst { get => _Inst; }

        private NetworkManager() { }

  
        /// /////////////////////// ///////////////////////////////////


        private string server_ip = "192.168.3.2";
        private int server_port = 9400;

        private Socket server_Socket;

        /// <summary>
        /// 主要用于创建新的线程的主要线程
        /// </summary>
        private Task _ListenTask;

        private CancellationToken _cancellationToken;

        private Dictionary<int, Task> _TaskDic = new Dictionary<int, Task>();
        private Dictionary<int, CilentNetManager> _CilentDic = new Dictionary<int, CilentNetManager>();


        private int server_index = 0;
        private int cilent_index = 0;


        /// <summary>
        /// 设置主线程网络信息
        /// </summary>
        /// <param name="server_ip"></param>
        /// <param name="server_port"></param>
        /// <param name="server_Socket"></param>
        /// <param name="listenTask"></param>
        /// <param name="cancellationToken"></param>
        public void SetInfo(string server_ip, int server_port, Socket server_Socket, Task listenTask, CancellationToken cancellationToken)
        {
            this.server_ip = server_ip;
            this.server_port = server_port;
            this.server_Socket = server_Socket;
            _ListenTask = listenTask;
            _cancellationToken = cancellationToken;
        }



        /// <summary>
        /// 添加一个客户端连接
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="index">标记的客户端号</param>
        /// <returns> 返回index 失败返回 -1 </returns>
        public int AddClient(Socket connect,int index)
        {
            if (_TaskDic.TryGetValue(index,out Task cilentTask))
            {
                index = -1;
            }
            else
            {
                cilentTask = new Task(()=> {
                    ClientServerStart(connect, index);
                });
                _TaskDic[index] = cilentTask;

                cilentTask.Start();

            }



            return index;
        }



        /// <summary>
        /// 接收来自客户端的消息
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="index"></param>
        private void ClientServerStart(Socket connect, int index)
        {
            // 创建clitent
            NetFunc netFunc = new NetFunc();

            CilentNetManager cilent = new CilentNetManager(connect, index, netFunc);
            _CilentDic[index] = cilent;

            cilent.Start();

            while (true)
            {
                if (cilent.IsConnect)
                {
                    if (connect.Available > 0)
                    {
                        byte[] bytes = new byte[1024]; // 确保开始只取头部
                        int nRead = connect.Receive(bytes);
                        if (nRead > 0)
                        {
                            ReceiverAllMessage(bytes, connect, nRead, netFunc);
                        }
                        else
                        {
                            Console.WriteLine("");
                        }

                    }
                }
                else
                {
                    Console.WriteLine("客户端[{0}]:{1}断开连接",cilent.id,cilent.GetCilentName());

                    cilent.Close();

                    _TaskDic.Remove(index);
                    break;
                }
                


            }

            connect.Disconnect(true);


        }


        /// <summary>
        /// 接收客户端的完整数据信息，
        /// </summary>
        /// <param name="bytes">已经提前取出的数据，没有则为null</param>
        /// <param name="connect">客户端的连接，继续读取数据</param>
        private void ReceiverAllMessage(byte[] bytes, Socket connect,int receiveLen, NetFunc netFunc)
        {
          
            lock(connect){
                if (bytes == null)
                {
                    bytes = new byte[1024];
                    receiveLen = connect.Receive(bytes);
                    if (receiveLen < 1)
                    {
                        Console.WriteLine("这次没读取到数据");
                        return;
                    }
                }

                // 获得表头
                HeadMessageMod head = NetUtils.Inst.GetHeadMessageMod(bytes);
                int messageLength;

                if (head.totalLength < 1)
                {
                    Console.WriteLine("读到表头信息信息为 null");
                    return;
                }

                if (receiveLen < MessageParamDefine.TABLE_HEAD_LENGTH)
                {
                    Console.Error.WriteLine("实际获取长度，小于表头长度");
               
                    return;
                }

                // 表示接收到数据
                Console.WriteLine("收到客户端index:{0},发来的消息，服务器index:{1}",head.cmid,head.smid);

                // 接着开始获取数据--接着解析
                messageLength = head.totalLength - MessageParamDefine.TABLE_HEAD_LENGTH;

                // 取出剩余部分
                // 下面获得 msg的byte部分
                List<byte> byteList = null;
                byte[] resultMsgs = null;

                resultMsgs = new byte[messageLength];
                for (int i = 0,j = MessageParamDefine.TABLE_HEAD_LENGTH;  i < messageLength; i++)
                {
                    resultMsgs[i] = bytes[j++];

                    if (i == messageLength - 1)
                    {
                        Console.WriteLine("");
                    }
                }

                // 先读取第一段
                if ((receiveLen - MessageParamDefine.TABLE_HEAD_LENGTH) >= messageLength) // 如果收到数据足够了，就不继续取了
                {
                   
                }
                else
                {
                    // 先获取前半部分
                    byteList = new List<byte>(resultMsgs);

                    try
                    {
                        while (messageLength > 0)
                        {

                            if (connect.Available > 0) // 有数据读取
                            {
                                // 取需要的部分
                                receiveLen = connect.Receive(bytes,messageLength,SocketFlags.None);
                                if (messageLength >= receiveLen)
                                {
                                    if (receiveLen == 1024)
                                    {
                                        byteList.AddRange(bytes);
                                    }
                                    else
                                    {
                                        resultMsgs = new byte[messageLength];
                                        Array.Copy(bytes, resultMsgs, receiveLen);

                                        byteList.AddRange(resultMsgs);
                                    }
                                }

                                messageLength -= receiveLen;

                            }
                            else
                            {
                                if (messageLength > 0)
                                {
                                    Console.Error.WriteLine("队列没有数据，但读到的数据长度不够");
                                    return;
                                }
                                break;
                            }
                        }


                        // 生成数组

                        resultMsgs = byteList.ToArray();
                    }
                    catch (SocketException e)
                    {
                        Console.Error.WriteLine("错误代码：" + e.ErrorCode);
                    }
                }

                // 可以解析了

                string msg = System.Text.Encoding.UTF8.GetString(resultMsgs);

                NetProtocolMod protocolMod = JsonUtils.Inst.ConvertJson2NetProtocolMod(msg);
                protocolMod.ConvertMsg();// 必须将msg 转换一下

                // 发送回调
                netFunc.CallBack((MsgId)protocolMod.msgId, protocolMod.msg);

                Console.WriteLine("收到客户端{0}，第{1}，上次服务器{2}，消息协议{3}，Time:{4}",
                    protocolMod.cilentId,
                    head.cmid,
                    head.smid,
                    ((MsgId)(protocolMod.msgId)).ToString(),
                    DateTime.Now);
            }
        }
    }
}
