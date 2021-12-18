using MyServer.Message;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MyServer.NetWork
{
    /// <summary>
    /// 管理客户端的网络逻辑
    /// </summary>
    partial class CilentNetManager
    {
        private Socket _Connent;

        /// <summary>
        /// 对应客户端id
        /// </summary>
        public int id;

        /// <summary>
        /// 客户端协议发送对象
        /// </summary>
        private NetFunc netFunc;

        /// <summary>
        /// 登录信息
        /// </summary>
        private DLoginData loginData;


       
        /// <summary>
        /// 连接对象
        /// </summary>
        private bool _isConnect = false;

        public bool IsConnect { get => _isConnect; }

        public CilentNetManager(Socket connent, int id, NetFunc netFunc)
        {
            _Connent = connent;
            this.id = id;
            this.netFunc = netFunc;

            _isConnect = true;
        }



        public void Start()
        {
            RegProtol();
        }


        public void Close()
        {
            UnRegProtol();
        }


        public void Update()
        {

        }


        /// <summary>
        /// 获得客户端的名称
        /// </summary>
        /// <returns></returns>
        public string GetCilentName()
        {

            return loginData?.cilentName;
        }

       
    }
}
