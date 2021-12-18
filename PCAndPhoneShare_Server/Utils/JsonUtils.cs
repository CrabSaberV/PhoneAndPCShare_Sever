using System;
using System.Collections.Generic;
using System.Text;
using MyServer.Message;
using Newtonsoft.Json;

namespace MyServer.NetWork
{
    class JsonUtils
    {
        private static JsonUtils _Inst = new JsonUtils();

        private JsonUtils() { }

        public static JsonUtils Inst { get => _Inst;}


        /// <summary>
        /// 反序列化 json 为 massage 的 obj
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ConvertJson2MessageMod(string msg,Type type)
        {

            return JsonConvert.DeserializeObject(msg,type);
        }


        /// <summary>
        /// 序列化 协议mod
        /// </summary>
        /// <param name="netData"></param>
        public string ConvertNetProtocolMod2Json(NetProtocolMod netData)
        {


            string a = JsonConvert.SerializeObject(netData);

            return a;

        }

        /// <summary>
        /// 反序列化 json 为 NetProtocolMod
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public NetProtocolMod ConvertJson2NetProtocolMod(string msg)
        {
        
            return JsonConvert.DeserializeObject<NetProtocolMod>(msg);

        }


    }
}
