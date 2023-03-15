using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using RD91S.tools;
using Reader;

namespace RD91S
{
    internal class SingleDevice
    {
       
       /// <summary>
       /// 当前刷卡人员
       /// </summary>
        public string currentManCode { get; set; }
        /// <summary>
        /// 是否开启循环读取数据
        /// </summary>
        public bool isLoop = false;
        /// <summary>
        /// 错误信息
        /// </summary>
        private  string strException = string.Empty;
        /// <summary>
        /// 串口号
        /// </summary>
        string strComPort;
        /// <summary>
        /// 波特率
        /// </summary>
        int nBaudrate = 115200;
        public ReaderMethod reader { get; }
        /// <summary>
        /// 单例模式
        /// </summary>
        private static SingleDevice singleDevice = new SingleDevice();
        private SingleDevice()
        {
            INIParser ini = new INIParser();
            ini.Open(@"config.ini");
            strComPort = ini.ReadValue("COM", "COM", "NULL");
            ini.Close();
            //strComPort = ConfigurationManager.AppSettings["COM"];
            reader = new ReaderMethod();
            currentManCode = string.Empty;
            reader.m_OnInventoryTag = onInventoryTag;
            reader.m_OnInventoryTagEnd = onInventoryTagEnd;
            reader.m_OnExeCMDStatus = onExeCMDStatus;
            reader.m_RefreshSetting = refreshSetting;
            reader.m_OnOperationTag = onOperationTag;
            reader.m_OnOperationTagEnd = onOperationTagEnd;
            reader.m_OnFastSwitchAntInventoryTagEnd = onFastSwitchAntInventoryTagEnd;
            reader.m_OnGetInventoryBufferTagCount = onGetInventoryBufferTagCount;
            reader.m_OnInventory6BTag = onInventory6BTag;
            reader.m_OnInventory6BTagEnd = onInventory6BTagEnd;
            reader.m_OnRead6BTag = onRead6BTag;
            reader.m_OnWrite6BTag = onWrite6BTag;
            reader.m_OnLock6BTag = onLock6BTag;
            reader.m_OnLockQuery6BTag = onLockQuery6BTag;
            reader.ReceiveCallback = onReceiveCallback;
        }
        public static SingleDevice getInstance()  { return singleDevice; }
        public bool connect()
        {
            //Processing serial port to connect reader.
           
            int nRet = reader.OpenCom(strComPort, nBaudrate, out strException);
            if (nRet != 0)
            {
                string strLog = "Connection failed, failure cause: " + strException;
                Console.WriteLine(strLog);
                return false;
            }
            else
            {
                string strLog = "Connect" + strComPort + "@" + nBaudrate.ToString();
                Console.WriteLine("读写器已连接:"+strLog);
                return true;
            }
        }

        public bool startInventoryReal() 
        {
            if (reader.InventoryReal((byte)0xFF, (byte)0xFF) != 0)
            {
                return false;
            }
            else {
                isLoop = true;
                return true;
            }
           
           
        }

        void onReceiveCallback(byte[] btAryReceiveData)
        {
            string str = "";
            for (int i = 0; i < btAryReceiveData.Length; i++)
            {
                str += Convert.ToString(btAryReceiveData[i], 16) + "  ";
            }
            //注释掉没用的输出！
            Console.WriteLine("cmd data ： " + str);
        }

        void refreshSetting(ReaderSetting readerSetting)
        {
            Console.WriteLine("Version:" + readerSetting.btMajor + "." + readerSetting.btMinor);
        }

        void onExeCMDStatus(byte cmd, byte status)
        {
            if (isLoop && (cmd == CMD.REAL_TIME_INVENTORY))
            {
                reader.InventoryReal((byte)0xFF, (byte)0xFF);
            }
            Console.WriteLine("CMD execute CMD:" + CMD.format(cmd) + "++Status code:" + ERROR.format(status));
        }

        void onInventoryTag(RXInventoryTag tag)
        {
            string tagData = tag.strEPC.Replace(" ", "").Substring(0, 11);
            Console.WriteLine("data:",tag.strEPC);
            if (DataChaeck.isHandset(tagData))
            {
                currentManCode = tagData;
            }
            else
            {
                currentManCode=string.Empty;
            }
            
            Console.WriteLine("Inventory Ant:" + tag.btAntId);
        }

        void onInventoryTagEnd(RXInventoryTagEnd tagEnd)
        {
            if (isLoop)
            {
                reader.InventoryReal((byte)0xFF, (byte)0xFF);
            }
        }

        void onFastSwitchAntInventoryTagEnd(RXFastSwitchAntInventoryTagEnd tagEnd)
        {
            Console.WriteLine("Fast Inventory end:" + tagEnd.mTotalRead);
        }

        void onInventory6BTag(byte nAntID, String strUID)
        {
            Console.WriteLine("Inventory 6B Tag:" + strUID);
        }

        void onInventory6BTagEnd(int nTagCount)
        {
            Console.WriteLine("Inventory 6B Tag:" + nTagCount);
        }

        void onRead6BTag(byte antID, String strData)
        {
            Console.WriteLine("Read 6B Tag:" + strData);
        }

        void onWrite6BTag(byte nAntID, byte nWriteLen)
        {
            Console.WriteLine("Write 6B Tag:" + nWriteLen);
        }

        void onLock6BTag(byte nAntID, byte nStatus)
        {
            Console.WriteLine("Lock 6B Tag:" + nStatus);
        }

        void onLockQuery6BTag(byte nAntID, byte nStatus)
        {
            Console.WriteLine("Lock query 6B Tag:" + nStatus);
        }

        void onGetInventoryBufferTagCount(int nTagCount)
        {
            Console.WriteLine("Get Inventory Buffer Tag Count" + nTagCount);
        }

        void onOperationTag(RXOperationTag tag)
        {
            Console.WriteLine("Operation Tag" + tag.strData);
        }

        void onOperationTagEnd(int operationTagCount)
        {
            Console.WriteLine("Operation Tag End" + operationTagCount);
        }



    }
}
