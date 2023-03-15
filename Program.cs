using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RD91S.tools;
using Reader;

namespace RD91S
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //读卡器连接
            SingleDevice device = SingleDevice.getInstance();
            if (device.connect())
            {
                device.isLoop = true;
            }
            else
            {
                Console.WriteLine("读卡器连接失败！");
            }
            Console.WriteLine("[服务器开启]");
            RD91sServer server=RD91sServer.getInstance();
            server.listen();
            while (true) 
            {
               
                //握手阻塞
                Socket client = server.accpet();
                //接收阻塞
                if (server.recive(client)=="ManCode") 
                {
                   
                    //Send
                    byte[] bytes = System.Text.Encoding.Default.GetBytes( "phone"+device.currentManCode);
                    client.Send(bytes);
                   
                    Console.WriteLine("当前牧民："+device.currentManCode);
                    device.currentManCode=String.Empty;
                    
                }
                //开启设备盘点
                device.reader.InventoryReal((byte)0xFF, (byte)0xFF);

            }
        }
    }
}
