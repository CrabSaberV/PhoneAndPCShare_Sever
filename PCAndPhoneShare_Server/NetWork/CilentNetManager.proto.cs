using MyServer.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer.NetWork
{
    /// <summary>
    /// 管理客户端的网络逻辑
    /// </summary>
    partial class CilentNetManager
    {

        /// <summary>
        /// 注册协议
        /// </summary>
        public void RegProtol()
        {
            netFunc.AddFunc(MsgId.CSLogin, OnCSLogin);
            netFunc.AddFunc(MsgId.CSCilentDisConnet, OnCSCilentDisConnet);
        }


        /// <summary>
        /// 注销协议
        /// </summary>
        public void UnRegProtol()
        {
            netFunc.RemoveFunc(MsgId.CSLogin, OnCSLogin);
            netFunc.RemoveFunc(MsgId.CSCilentDisConnet, OnCSCilentDisConnet);
        }


        /// <summary>
        /// 客户端登录协议
        /// </summary>
        /// <param name="msg"></param>
        private void OnCSLogin(object msg)
        {
            
            CSLogin obj = (CSLogin)msg;

            loginData = obj.data;

            Console.WriteLine("有客户端登陆了:" + obj.data.cilentName);

        }


        /// <summary>
        /// 当客户端断开
        /// </summary>
        /// <param name="msg"></param>
        private void OnCSCilentDisConnet(object msg)
        {
            CSCilentDisConnet obj = msg as CSCilentDisConnet;

            if (obj.isDisConnet)
            {
                _isConnect = false;
            }
            else
            {
                Console.Error.WriteLine("什么情况,客户端要断开连接，却发来否定信息");
            }
        }
    }
}
