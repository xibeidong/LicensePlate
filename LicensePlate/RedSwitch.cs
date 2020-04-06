using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LicensePlate
{
    public class RedSwitch
    {
        public delegate void UpdataRedSwitchUIDelegate(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8);
        public UpdataRedSwitchUIDelegate UpdateRedSwitchUI;

        SerialPort m_serialPort_RedSwitch = new SerialPort();
       
        int n1, n2, n3, n4, n5, n6, n7, n8;//n1-n4 入厂围栏；n5-n8 出厂围栏
       
        System.Timers.Timer redSwitchTimer1;

        public void OpenDevice()
        {
            string str_com = IniFiles.iniFile.IniReadValue("redswitch", "port1");
            string[] names = SerialPort.GetPortNames();
            bool hasPort = false;
            foreach (string item in names)
            {
                if (item == str_com)
                {
                    hasPort = true;
                }
            }
            if (!hasPort)
            {
                Log.myLog.Warn("没有发现串口：" + str_com + " open failed!");
                MessageBox.Show("没有发现串口：" + str_com + " open failed!");
                return;
            }
            m_serialPort_RedSwitch.PortName = str_com; //"COM6";
            m_serialPort_RedSwitch.StopBits = StopBits.One;
            m_serialPort_RedSwitch.Parity = Parity.None;

            m_serialPort_RedSwitch.BaudRate = 9600;
            m_serialPort_RedSwitch.DataBits = 8;
            m_serialPort_RedSwitch.DtrEnable = true;
            m_serialPort_RedSwitch.RtsEnable = true;
            m_serialPort_RedSwitch.ReceivedBytesThreshold = 1;
            m_serialPort_RedSwitch.ReadBufferSize = 4096;
            m_serialPort_RedSwitch.WriteTimeout = 100000;
            m_serialPort_RedSwitch.ReadTimeout = 100000;
            m_serialPort_RedSwitch.Open();
            m_serialPort_RedSwitch.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived_redSwitch1);

            Thread.Sleep(500);
            Log.myLog.Info("串口打开成功:" + str_com);

            redSwitchTimer1 = new System.Timers.Timer(500);//实例化Timer类，设置间隔时间为1000毫秒；

            redSwitchTimer1.Elapsed += new System.Timers.ElapsedEventHandler(timerTheout1);//到达时间的时候执行事件；

            redSwitchTimer1.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

            redSwitchTimer1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；



            System.Timers.Timer tt = new System.Timers.Timer(750);
            tt.Elapsed += new System.Timers.ElapsedEventHandler(timerTheout2);//到达时间的时候执行事件；

            tt.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

            tt.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；


        }

        private void timerTheout2(object source, System.Timers.ElapsedEventArgs e)
        {
            if (checkD01)
            {
                Console.WriteLine("定时器触发，D01");
                DropIn();
            }
            else if (checkD02)
            {
                Console.WriteLine("定时器触发，D02");
                DropOut();
            }
        }
        private void timerTheout1(object source, System.Timers.ElapsedEventArgs e)
        {
            byte[] data = AddCRC(new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10 });//0x01设备地址，0x02 查询指令，0x00,0x00起始地址，0x01,0x00 查询状态数量
            m_serialPort_RedSwitch.Write(data, 0, data.Length);

          
        }

        bool checkD01 = false;
        //检查第一路信号关闭返回
        void Check_D01(byte[] data)
        {
            if (!checkD01)
            {
                return;
            }
            byte[] checkData = new byte[] { 0x01, 0x05, 0x00, 0x00, 0x00, 0x00, 0xCD, 0xCA };
            if (data.Length == 8)
            {
                for (int i = 0; i < 6; i++)
                {
                   
                        if (checkData[i] != data[i])
                        {
                            return;
                        } 
                    
                }
                checkD01 = false;
                Console.WriteLine("Do1 关闭成功");
            }
        }
        bool checkD02 = false;
        void Check_D02(byte[] data)
        {
            if (!checkD02)
            {
                return;
            }
            byte[] checkData = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };//还有两位校验码
            if (data.Length == 8)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (true)
                    {
                        if (checkData[i] != data[i])
                        {
                            return;
                        }
                    }
                }
                checkD02 = false;
                Console.WriteLine("Do2 关闭成功");
            }
        }

        private void Serial_DataReceived_redSwitch1(object sender, SerialDataReceivedEventArgs e)
        {

            int intByteCount = m_serialPort_RedSwitch.BytesToRead;
            byte[] bytes = new byte[intByteCount];
            m_serialPort_RedSwitch.Read(bytes, 0, intByteCount);
            //strReceiveBuf += Encoding.Default.GetString(bytes);
            // Log.myLog.Info("红外串口接收：" + intByteCount);
            // FE 02 02 00 00 AD AC
            if (intByteCount < 7)
            {
                return;
            }
            Check_D01(bytes);
            Check_D02(bytes);

            if (bytes[0] == 0x01)//设备地址
            {
                if (bytes[1] == 0x02)//0x02 查询指令，0x82 查询错误
                {

                    if (bytes[2] == 0x02)
                    {


                        n8 = (bytes[3] & 0x80) == 0x80 ? 1 : 0;
                        n7 = (bytes[3] & 0x40) == 0x40 ? 1 : 0;
                        n6 = (bytes[3] & 0x20) == 0x20 ? 1 : 0;
                        n5 = (bytes[3] & 0x10) == 0x10 ? 1 : 0;
                        n4 = (bytes[3] & 0x08) == 0x08 ? 1 : 0;
                        n3 = (bytes[3] & 0x04) == 0x04 ? 1 : 0;
                        n2 = (bytes[3] & 0x02) == 0x02 ? 1 : 0;
                        n1 = (bytes[3] & 0x01) == 0x01 ? 1 : 0;
                        //Console.WriteLine("n1="+n1);
                        //Console.WriteLine("n2=" + n2);
                        //Console.WriteLine("n3=" + n3);
                        //Console.WriteLine("n4=" + n4);
                        //Console.WriteLine("n5=" + n5);
                        //Console.WriteLine("n6=" + n6);
                        //Console.WriteLine("n7=" + n7);
                        //Console.WriteLine("n8=" + n8);

                        UpdateRedSwitchUI(n1, n2, n3, n4, n5, n6, n7,n8);
                        // 入厂围栏
                        if (n1 > 0 && n2 > 0 && n3 > 0 && n4 > 0)
                        {
                           Manager.instance.m_inRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.instance.m_inRedSwitchOK = false;
                        }
                      
                      
                        //出厂围栏
                        if (n5 > 0 && n6 > 0 && n7 > 0 && n8 > 0)
                        {
                            Manager.instance.m_outRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.instance.m_outRedSwitchOK = false;
                        }
                    }
                }
            }

        }

        public void RiseIn()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A };
            m_serialPort_RedSwitch.Write(data, 0, data.Length);
            checkD01 = true;
            //Thread.Sleep(350);
            //DropIn();
        }
        public void DropIn()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x00, 0x00, 0x00, 0xCD, 0xCA };
            m_serialPort_RedSwitch.Write(data, 0, data.Length);
            Console.WriteLine("Do1 发送关闭指令");
        }
        public void RiseOut()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA };
            m_serialPort_RedSwitch.Write(data, 0, data.Length);
            checkD02 = true;
            //Thread.Sleep(350);
            //DropIn();
        }
        public void DropOut()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };
            data = AddCRC(data);
            m_serialPort_RedSwitch.Write(data, 0, data.Length);
        }

        byte[] AddCRC(byte[] data)
        {
            byte[] r = new byte[2 + data.Length];
            var b = CRC.CRC16(data);
            for (int i = 0; i < data.Length; i++)
            {
                r[i] = data[i];
            }
            r[data.Length] = b[1];
            r[data.Length + 1] = b[0];
            return r;
        }
    }
}
