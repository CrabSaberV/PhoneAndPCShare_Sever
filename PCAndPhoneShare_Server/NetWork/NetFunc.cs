
using MyServer.NetWork.DelegateFunc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer.NetWork
{
    /// <summary>
    /// 用于网络数据的回调/发送 (要考虑到每个客户端)
    /// </summary>
    class NetFunc
    {
        private Dictionary<int,MsgCallBack> callBackDic = new Dictionary<int, MsgCallBack>();

        /// <summary>
        /// 添加网络回调 （一般来自客户端的 CS协议）
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="callBack"></param>
        public void AddFunc(MsgId msgId , MsgCallBack callBack)
        {
            MsgCallBack func;
            if (callBackDic.TryGetValue((int)msgId,out func))
            {
                func += callBack;
            }
            else
            {
                callBackDic[(int)msgId] = callBack;
            }
        }

        /// <summary>
        /// 移除一个 func
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="callBack"></param>
        public void RemoveFunc(MsgId msgId, MsgCallBack callBack)
        {
            MsgCallBack func;
            if (callBackDic.TryGetValue((int)msgId, out func))
            {
                func -= callBack;
            }
           
        }



        /// <summary>
        /// 来自客户端的 事件回调
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg">json 转化后的 object </param>
        public void CallBack(MsgId msgId,object msg)
        {
            MsgCallBack func;
            if (callBackDic.TryGetValue((int)msgId, out func))
            {
                func.Invoke(msg);
            }
            
        }


       
        
    }
}
