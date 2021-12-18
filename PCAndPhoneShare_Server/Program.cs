
using MyServer.NetWork;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer
{

    [Serializable]
    class Program
    {

        private string server_ip = "192.168.3.2";
        private int server_port = 9400;

        private Socket server_Socket;

        /// <summary>
        /// 主要用于创建新的线程的主要线程
        /// </summary>
        private Task _ListenTask;

        private CancellationToken _cancellationToken;

        private int Index = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("服务器！！");
            Program program = new Program();

            program.ShowServerIP();

            program.StartNet();


            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.K)
                {
                    Console.WriteLine("结束");
                    break;
                }
            }
        }


        /// <summary>
        /// 显示服务器ip
        /// </summary>
        private void ShowServerIP()
        {
            try
            {
                string name = Dns.GetHostName();
                IPAddress[] ipaddressList = Dns.GetHostAddresses(name);

                foreach (var addr in ipaddressList)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine(addr.ToString());
                    }
                }
            }catch (Exception e)
            {
                Console.WriteLine("出错" + e.Message);
            }
        }


        /// <summary>
        ///  开启网络连接，监听连接
        /// </summary>
        public void StartNet()
        {
            server_Socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(server_ip);
            IPEndPoint point = new IPEndPoint(ip, server_port);

            // 绑定监听端口
            server_Socket.Bind(point);
            server_Socket.Listen(2); // 目前运行监听2个

            Console.WriteLine("监听窗口成功，ip:{0} port:{1}",server_ip,server_port);

            


            _cancellationToken = new CancellationToken(false);
            _ListenTask = new Task(ListenWork, _cancellationToken);

            // 设置 网络管理器
            NetworkManager.Inst.SetInfo(server_ip, server_port, server_Socket, _ListenTask, _cancellationToken);

            _ListenTask.Start();
        }


        /// <summary>
        /// 监听工作，监听来自客户端的连接，成功后，创建线程来管理
        /// </summary>
        private void ListenWork()
        {
            while (true)
            {
                Socket _connect = server_Socket.Accept();

                // 连接成功
                Console.WriteLine("连接到一个客户端，data:{0}",_connect.RemoteEndPoint.ToString());

                NetworkManager.Inst.AddClient(_connect, Index++);
            }
        }
    }
}
