using MyServer.NetWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer.Message
{
    /// <summary>
    /// 封装的协议结构，主要用于发送和接收
    /// </summary>
    [Serializable]
    class NetProtocolMod
    {
        public int msgId;

        public int cilentId;

        public object msg;

        public void ConvertMsg()
        {
            string a = msg.ToString();
            Type type = null;
            if (MessageModDef.MessageIdDic.TryGetValue(msgId,out type))
            {
                msg = JsonUtils.Inst.ConvertJson2MessageMod(a, MessageModDef.MessageIdDic[msgId]);
            }
            else
            {
                Console.Error.WriteLine("请检查 MsgId 是否存在，MsgId : {0}",msgId);
            }
            
        }

    }



    /// <summary>
    /// 表头结构
    /// </summary>
    struct HeadMessageMod
    {
        /// <summary>
        /// 信息总长
        /// </summary>
        public int totalLength;
        
        /// <summary>
        /// 客户端的消息index
        /// </summary>
        public int cmid;

        /// <summary>
        /// 服务器的消息index
        /// </summary>
        public int smid;

        public HeadMessageMod(int totalLength, int cmid, int smid)
        {
            this.totalLength = totalLength;
            this.cmid = cmid;
            this.smid = smid;
        }


       
    }
}
