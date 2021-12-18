using MyServer.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer
{
   /// <summary>
   /// 定义的常量
   /// </summary>
    class MessageParamDefine
    {
        /// <summary>
        /// 表头长度  总长 + cmid + smid
        /// </summary>
        public static  readonly int  TABLE_HEAD_LENGTH = 12;


       
    }



    /// <summary>
    /// 网络分析的一些工具
    /// </summary>
    class NetUtils
    {

        private static NetUtils _Inst = new NetUtils(); 

        private NetUtils() { }

        public static NetUtils Inst { get => _Inst; }


        /// <summary>
        /// 获得表头信息
        /// </summary>
        /// <param name="bytes">信息bytes</param>
        /// <returns></returns>
        public HeadMessageMod GetHeadMessageMod(byte[] bytes)
        {
            int length = 0;
            int cmid = 0;
            int smid = 0;

            int index = 0;
            byte[] temp = new byte[4];

            // 获得总长
            for ( int i = 0; i < 4 ; i ++)
            {
                temp[i] = bytes[index++];
            }

            length = BitConverter.ToInt32(temp);

            // 获得cmid
            for (int i = 0; i < 4; i++)
            {
                temp[i] = bytes[index++];
            }
            cmid = BitConverter.ToInt32(temp);

            // 获得cmid
            for (int i = 0; i < 4; i++)
            {
                temp[i] = bytes[index++];
            }
            smid = BitConverter.ToInt32(temp);

            return new HeadMessageMod(length, cmid, smid);
        }
    }
}
