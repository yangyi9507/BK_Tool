using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BK_Tool
{
    public class SockectComm
    {
        public static string IpString = ConfigurationManager.AppSettings["IpString"];
        public static string PortString = ConfigurationManager.AppSettings["PortString"];
        
        private static Socket tcpServer = null;
        private static Socket clientSocket = null;
        public byte[] buffer = new byte[1024 * 1024 * 3];
        public static List<TcpClient> clientList = new List<TcpClient>();
        
        public SockectComm()
        {
            Ini();
        }

        public void Ini()
        {
            if (tcpServer != null) { tcpServer = null; }
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse(IpString);
            EndPoint point = new IPEndPoint(ipaddress, int.Parse(PortString));

            tcpServer.Bind(point);//绑定IP和申请端口
            tcpServer.Listen(100);//设置客户端最大连接数
            //tcpServer.BeginAccept(new AsyncCallback(this.AsyncAcceptSocket), this);
            clientSocket = tcpServer.Accept();

            Console.WriteLine((clientSocket.RemoteEndPoint as IPEndPoint).Address + "已连接");
            clientSocket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), this);
        }

        #region   接收数据
        /// <summary>
        /// 接收数据部分
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    int bytesRead = clientSocket.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        byte[] haveDate = ByteToByte(buffer, bytesRead, 0);//取出有效的字节数据,得到实际的数据haveDate
                        Array.Clear(buffer, 0, buffer.Length);//情况缓冲数组

                        string strAll = TextEncoder.bytesToText(haveDate);

                        //ChuLiOneHL7(strAll);//处理数据
                        SFLReportMain sflreportMain = new SFLReportMain();
                        sflreportMain.ChuLiOneHL7(strAll);

                        strAll = "";
                        clientSocket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), this);//接收剩余的数据或者继续接收数据
                    }
                    else //如果接收到数据字符为0，代表接收完了
                    {
                        tcpServer.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), this);//继续接收数据
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }
        #endregion

        #region 连接回调
        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);

                client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), this);
                Console.WriteLine("Socket connect to {0}", client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());                
            }
        }
        #endregion

        #region 把一个数组取出指定的长度
        /// <summary>
        /// 把一个数组取出指定的长度
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] ByteToByte(byte[] a, int b, int index)
        {
            byte[] haveDate = new byte[b];
            Array.Copy(a, index, haveDate, 0, b);
            return haveDate;
        }
        #endregion

    }
}
