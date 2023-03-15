using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace RD91S.tools
{
    internal class RD91sServer
    {
        //Socket
        private Socket listenfd;
        //Bind
        private IPAddress ipAdr;
        private IPEndPoint ipEp;
        //单例
        private static RD91sServer Instance=new RD91sServer();
        private RD91sServer() 
        {
            //绑定地址和端口号
            listenfd = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Stream, ProtocolType.Tcp);
            ipAdr = IPAddress.Parse("127.0.0.1");
            ipEp = new IPEndPoint(ipAdr, 12138);
            listenfd.Bind(ipEp);
        }

        public static RD91sServer  getInstance() 
        {
            return Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        public void listen() 
        {
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
        }
        /// <summary>
        /// 握手
        /// </summary>
        public Socket accpet() 
        {
            //Accept
            Socket connfd = listenfd.Accept();
            Console.WriteLine("[服务器]Accept");
            return connfd;
        }
        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="connfd"></param>
        public string recive(Socket connfd) 
        {
            //Recv
            byte[] readBuff = new byte[1024];
            int count = connfd.Receive(readBuff);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            Console.WriteLine("[服务器接收]" + str);
            return str;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        public void send(Socket connfd,string message) 
        {
            //Send
            byte[] bytes = System.Text.Encoding.Default.GetBytes("serv echo " +message );
            connfd.Send(bytes);
        }
    }
}
