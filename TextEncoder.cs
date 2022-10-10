using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BK_Tool
{
    /// <summary>
    /// 转换工具类
    /// </summary>
    public sealed class TextEncoder
    {
        /// <summary>
        /// byte数组转换为string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToText(byte[] bytes)
        {
            return Encoding.GetEncoding("UTF-8").GetString(bytes);
            //return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] textToBytes(string text)
        {
            return Encoding.GetEncoding("gb2312").GetBytes(text); ;
            //byte[] buf = new byte[0];
            //if (!string.IsNullOrEmpty(text))
            //{
            //    buf = Encoding.UTF8.GetBytes(text);
            //}
            //return buf;
        }

        ///// <summary>
        ///// byte数组转换为string
        ///// </summary>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public static string bytesToGb2312(byte[] bytes)
        //{
        //    return Encoding.GetEncoding("gb2312").GetString(bytes);

        //}

        //public static byte[] Gb2312ToBytes(string text)
        //{
        //    return Encoding.GetEncoding("gb2312").GetBytes(text); ;
        //}
        #region 整数与byte[] 间的转换
        /// <summary>
        /// byte数组转换为int
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int bytesToInt(byte[] bytes)
        {
            int i = 0;
            try
            {
                //取前两个字节,做为长度
                if (bytes.Length < 4)
                {
                    byte[] buf = new byte[4];

                    for (int m = bytes.Length; m < buf.Length; m++)
                    {
                        buf[m] = 0;
                    }

                    Buffer.BlockCopy(bytes, 0, buf, 0, bytes.Length);//复制到buf数组中

                    i = BitConverter.ToInt32(buf, 0);
                }
                else
                {
                    i = BitConverter.ToInt32(bytes, 0);
                }

            }
            catch
            {
            }
            return i;
        }

        /// <summary>
        /// int转换为byte数组
        /// </summary>
        public static byte[] intToBytes(int i)
        {
            return BitConverter.GetBytes(i);
        }

        /// <summary>
        /// byte数组转换为无符号整数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static uint bytesToUInt32(byte[] bytes)
        {
            uint i = 0;

            try
            {
                i = BitConverter.ToUInt32(bytes, 0);
            }
            catch
            {
            }
            return i;
        }

        public static byte[] uintToBytes(uint i)
        {
            return BitConverter.GetBytes(i);
        }

        #endregion

        /// <summary>
        /// 传入整数获得最前面的2个字节(低位的2个字节)的16进制表示字符串，高位的2个字节丢掉，十进制10转换十六进制为000A
        /// 主机地址转换，长度转换时使用
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string intToHexByte(int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            return bytes[1].ToString("X2") + bytes[0].ToString("X2");
        }

        /// <summary>
        /// 传入字符数组获得最前面的2个字节并转换为整数，后面字节丢掉，12[高位] 25[低位] 36，则为：25[低位] 12[高位] 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int HexByteToint(string hexString)
        {
            byte[] bytes = HexStrToByte(hexString);

            int iResult = -1;
            if (bytes.Length >= 2)
            {
                byte[] returnBytes = new byte[2];

                //--前面的为高位，后面的为低位，进行兑换---
                returnBytes[0] = bytes[1];//低位
                returnBytes[1] = bytes[0];//高位

                iResult = bytesToInt(returnBytes);
            }
            else
            {
                iResult = bytesToInt(bytes);
            }

            return iResult;
        }

        #region byte[] 与16进制字节数组转换
        /// <summary>
        /// 16进制字符串转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStrToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += "0";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                try
                {
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                catch
                {
                    //如果有异常则用#显示，63 --? 

                    returnBytes[i] = 35;
                }
            }
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;

        }

        #endregion

        #region 获取校验位
        /**
         * 前面几位的byte[] 数组，返回最后校验位数值
         * @param msg
         * @return
         */

        /// <summary>
        /// 前面几位的byte[] 数组，返回最后校验位数值
        /// </summary>
        public static byte getCheckTotal(byte[] msg)
        {
            int total = 0;
            for (int i = 0; i < msg.Length; i++)
            {
                total += msg[i];
            }
            if (total > 255)
            {
                total = (total % 256);
            }
            //String t = Integer.toHexString(total);
            //if (t.length() == 1)
            //{
            //    t = "0" + t;
            //}
            //t = t.substring(t.length() - 2, t.length()).toUpperCase();
            //System.out.println(t+"校验位");

            byte checkTotal = BitConverter.GetBytes(total)[0];// HexByteTools.HexString2Bytes(t);
            return checkTotal;
        }

        /// <summary>
        /// 传入所有16进制字符串，返回校验位数值
        /// </summary>
        public static string getCheckHex(string Hexstr)
        {
            return getCheckTotal(HexStrToByte(Hexstr)).ToString("X2");
        }

        #endregion

        #region 备用
        //        1.请问c#中如何将十进制数的字符串转化成十六进制数的字符串

        // //十进制转二进制
        // Console.WriteLine("十进制166的二进制表示: "+Convert.ToString(166, 2));

        // //十进制转八进制
        // Console.WriteLine("十进制166的八进制表示: "+Convert.ToString(166, 8));

        // //十进制转十六进制
        // Console.WriteLine("十进制166的十六进制表示: "+Convert.ToString(166, 16));

        // //二进制转十进制
        // Console.WriteLine("二进制 111101 的十进制表示: "+Convert.ToInt32("111101", 2));

        // //八进制转十进制
        // Console.WriteLine("八进制 44 的十进制表示: "+Convert.ToInt32("44", 8));

        // //十六进制转十进制
        // Console.WriteLine("十六进制 CC的十进制表示: "+Convert.ToInt32("CC", 16));

        //2.在串口通讯过程中，经常要用到 16进制与字符串、字节数组之间的转换
        ////

        //private string StringToHexString(string s,Encoding encode)
        //{
        // byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
        //     string result = string.Empty;
        //     for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
        //     {
        //          result += "%"+Convert.ToString(b[i], 16);
        //     }
        //    return result;
        //}


        //private string HexStringToString(string hs, Encoding encode)
        //{
        //     //以%分割字符串，并去掉空字符
        //     string[] chars = hs.Split(new char[]{'%'},StringSplitOptions.RemoveEmptyEntries);
        //     byte[] b = new byte[chars.Length];
        //     //逐个字符变为16进制字节数据
        //     for (int i = 0; i < chars.Length; i++)
        //     {
        //          b[i] = Convert.ToByte(chars[i], 16);
        //     }
        //     //按照指定编码将字节数组变为字符串
        //     return encode.GetString(b);
        //}






        //从汉字转换到16进制
        ///// <summary>
        ///// 从汉字转换到16进制
        ///// </summary>
        ///// <param name="s"></param>
        ///// <param name="charset">编码,如"utf-8","gb2312"</param>
        ///// <param name="fenge">是否每字符用逗号分隔</param>
        ///// <returns></returns>
        //public static string ToHex(string s, string charset, bool fenge)
        //{
        //   if ((s.Length % 2) != 0)
        //   {
        //      s += " ";//空格
        //      //throw new ArgumentException("s is not valid chinese string!");
        //   }
        //   System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
        //   byte[] bytes = chs.GetBytes(s);
        //   string str = "";
        //   for (int i = 0; i < bytes.Length; i++)
        //   {
        //      str += string.Format("{0:X}", bytes[i]);
        //      if (fenge && (i != bytes.Length - 1))
        //      {
        //          str += string.Format("{0}", ",");
        //      }
        //    }
        //      return str.ToLower();
        //}


        //16进制转换成汉字
        /////<summary>
        ///// 从16进制转换成汉字
        ///// </summary>
        ///// <param name="hex"></param>
        ///// <param name="charset">编码,如"utf-8","gb2312"</param>
        ///// <returns></returns>
        //public static string UnHex(string hex, string charset)
        //{
        //  if (hex == null)
        //     throw new ArgumentNullException("hex");
        //  hex = hex.Replace(",", "");
        //  hex = hex.Replace("\n", "");
        //  hex = hex.Replace("\\", "");
        //  hex = hex.Replace(" ", "");
        //  if (hex.Length % 2 != 0)
        //  {
        //       hex += "20";//空格
        //  }
        // // 需要将 hex 转换成 byte 数组。
        //  byte[] bytes = new byte[hex.Length / 2];

        //  for (int i = 0; i < bytes.Length; i++)
        //  {
        //     try
        //      {
        //          // 每两个字符是一个 byte。
        //          bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
        //          System.Globalization.NumberStyles.HexNumber);
        //      }
        //      catch
        //      {
        //          // Rethrow an exception with custom message.
        //         throw new ArgumentException("hex is not a valid hex number!", "hex");
        //       }
        //   }
        //      System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
        //      return chs.GetString(bytes);
        //}



        #endregion
    }

}
