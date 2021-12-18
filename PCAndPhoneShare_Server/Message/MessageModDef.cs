// -@@  自动生成
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer.Message
{
    class MessageModDef
    {
        public static Dictionary<int, Type> MessageIdDic = new Dictionary<int, Type>
        {

            { 1001 ,typeof(CSLogin) },
            { 1002 ,typeof(SCLogin) },
            { 1003 ,typeof(CSCilentDisConnet) },
            { 1003 ,typeof(SCServerDisConnet) },
    
        };
    }
}
