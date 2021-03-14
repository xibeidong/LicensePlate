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
    public class CommandInfo
    {
        public CommdType commd_type;
        public byte[] mData;
    }

    public enum CommdType
    {
        InRise,
        InDrop,
        OutRise,
        OutDrop
    }
    public class RedSwitch
    {
        public delegate void UpdataRedSwitchUIDelegate(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8);
        public UpdataRedSwitchUIDelegate UpdateRedSwitchUI;

        SerialPort m_serialPort_RedSwitch = new SerialPort();
        int[] chanState = new int[16];
        int[] iniChan = new int[8]; //读取ini配置的，0-3入厂，4-7出厂
        int n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13, n14, n15, n16;//n1-n4 入厂围栏；n5-n8 出厂围栏,n5坏了，用n9

        System.Timers.Timer redSwitchTimer1;
        System.Timers.Timer redSwitchTimer2;


        List<byte> bufList = new List<byte>(64);

        bool checkD02open = false;
        bool checkD02close = false;
        bool checkD01open = false;
        bool checkD01close = false;

        public Queue<CommandInfo> commdQueue = new Queue<CommandInfo>();
        public Queue<byte[]> dataQueue = new Queue<byte[]>();
        bool isOpen = false;
        public void OpenDevice()
        {
            string str = IniFiles.iniFile.IniReadValue("redswitch", "in")+","+
               IniFiles.iniFile.IniReadValue("redswitch", "out");
            try
            {
                string[] strs = str.Split(',');
                for (int i = 0; i < 8; i++)
                {
                    iniChan[i] = int.Parse(strs[i])-1;
                }
            }
            catch (Exception )
            {
                MessageBox.Show("ini文件配置了错误的光耦通道");
                return;
            }
          
            

            //  Console.WriteLine(BitConverter.ToString(AddCRC(new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10 })));
            //byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };
            //data = AddCRC(data);
            //Console.WriteLine(BitConverter.ToString(data));
            if (isOpen==true)
            {
                Manager.Instance.LogToRichText("红外围栏设备不可以重复打开！"); 
                return;
            }

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
                MessageBox.Show("没有发现串口：" + str_com + "， 红外设备 open failed!");
                //Manager.instance.LogToRichText();
                Manager.Instance.LogToRichText("没有发现串口：" + str_com + "， 红外设备打开失败!");
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

            try
            {
                m_serialPort_RedSwitch.Open();
                m_serialPort_RedSwitch.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived_redSwitch1);
                // m_serialPort_RedSwitch.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
                Thread.Sleep(500);
                Log.myLog.Info("串口打开成功:" + str_com);
                Manager.Instance.LogToRichText("红外设备打开成功");
                isOpen = true;
                Thread th = new Thread(new ThreadStart(ParseData));
                th.IsBackground = true;
                th.Start();

                redSwitchTimer1 = new System.Timers.Timer(500);//实例化Timer类，设置间隔时间为1000毫秒；

                redSwitchTimer1.Elapsed += new System.Timers.ElapsedEventHandler(timerTheoutSendChannel);//到达时间的时候执行事件；

                redSwitchTimer1.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

                redSwitchTimer1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；



                redSwitchTimer1 = new System.Timers.Timer(1200);
                redSwitchTimer1.Elapsed += new System.Timers.ElapsedEventHandler(timerTheout2);//到达时间的时候执行事件；

                redSwitchTimer1.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

                redSwitchTimer1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                

            }
            catch (Exception e)
            {
                Manager.Instance.LogToRichText("红外设备打开失败，"+e.Message);
                
            }
            

        }
         ~RedSwitch()
        {
            if (redSwitchTimer1 != null)
            {
                redSwitchTimer1.Stop();
            }

            if (redSwitchTimer2 != null)
            {
                redSwitchTimer2.Stop();
            }
            if (m_serialPort_RedSwitch.IsOpen)
            {
                m_serialPort_RedSwitch.Close();
            }
        }

        void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)

        {
            Console.WriteLine("====> ERR:"+e.ToString());
            Console.WriteLine(e.EventType);

        }

        /// <summary>
        /// 校验开关闸是否成功，失败要继续发送开关命令
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void timerTheout2(object source, System.Timers.ElapsedEventArgs e)
        {
            if (checkD01close)
            {
                Console.WriteLine("定时器触发，send => D01关闭命令");
                DropIn();
            }
            else if (checkD02close)
            {
                Console.WriteLine("定时器触发，send => D02关闭命令");
                DropOut();
            }
            else if (checkD01open)
            {
                Console.WriteLine("定时器触发，send => D01打开命令");
                RiseIn();
            }
            else if (checkD02open)
            {
                Console.WriteLine("定时器触发，send => D02打开命令");
                RiseOut();
            }
        }
       /// <summary>
       /// 定时查询红外状态
       /// </summary>
       /// <param name="source"></param>
       /// <param name="e"></param>
        private void timerTheoutSendChannel(object source, System.Timers.ElapsedEventArgs e)
        {
            byte[] data;
           
            if (commdQueue.Count>0)
            {

                data = commdQueue.Dequeue().mData;
                
                Console.WriteLine("write done, commdQueue count = " + commdQueue.Count);
            }
            else
            {
                data = AddCRC(new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10 });//0x01设备地址，0x02 查询指令，0x00,0x00起始地址，0x01,0x00 查询状态数量
              
            }

            lock (m_serialPort_RedSwitch)
            {
                m_serialPort_RedSwitch.Write(data, 0, data.Length);
             
         
            }


        }
       
        //检查第一路信号关闭返回
        void Check_D01(byte[] data)
        {

            if (checkD01open)
            {
                byte[] checkData = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A };
                if (data.Length == 8)
                {
                    for (int i = 0; i < 6; i++)
                    {

                        if (checkData[i] != data[i])
                        {
                            return;
                        }

                    }
                    checkD01open = false;//已经成功打开，下一步关闭
                    checkD01close = true;
                    Console.WriteLine("D01成功打开");
                    return;
                }

             }


            if (checkD01close)
            {
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
                    checkD01close = false;
                    Console.WriteLine("Do1 关闭成功");
                }
            }
          
        }
       
        void Check_D02(byte[] data)
        {
            if (checkD02open)
            {
                //0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA
                byte[] checkData = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA };
                if (data.Length == 8)
                {
                    for (int i = 0; i < 6; i++)
                    {

                        if (checkData[i] != data[i])
                        {
                            return;
                        }

                    }
                    checkD02open = false;//已经成功打开，下一步关闭
                    checkD02close = true;
                    Console.WriteLine("D02成功打开");
                    return;
                }

            }


            if (checkD02close)
            {
                //0x01, 0x05, 0x00, 0x01, 0x00, 0x00
                byte[] checkData = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };//还有两位CRC校验码
                if (data.Length == 8)
                {
                    for (int i = 0; i < 6; i++)
                    {

                        if (checkData[i] != data[i])
                        {
                            return;
                        }

                    }
                    checkD02close = false;
                    Console.WriteLine("Do2 关闭成功");
                }
            }
          
        }

      
        private void Serial_DataReceived_redSwitch1(object sender, SerialDataReceivedEventArgs e)
        {
            lock (m_serialPort_RedSwitch)
            {
                //Console.WriteLine("port recv===>");
                int intByteCount = m_serialPort_RedSwitch.BytesToRead;
                byte[] bytes = new byte[intByteCount];

                m_serialPort_RedSwitch.Read(bytes, 0, intByteCount);
                dataQueue.Enqueue(bytes);

            }
          
        }

        void ParseData()
        {
            while (true)
            {

                if (dataQueue.Count > 0)
                {
                    byte[] bytes;

                    lock (dataQueue)
                    {
                        bytes = dataQueue.Dequeue();
                    }


                    bufList.AddRange(bytes);//放入缓存区
                                            // Console.WriteLine("bufList.Count = " + bufList.Count);
                  //  Console.WriteLine(BitConverter.ToString(bytes));



                    while (true) //有时候会突然收到几百个字节，必须全部处理掉
                    {

                       
                        if (bufList.Count >= 2)
                        {

                            if (bufList[0] != 0x01)
                            {
                                bufList.Clear();
                                Console.WriteLine("redSwitch port bufList clear");
                              
                            }
                            else if (bufList[1] != 0x02 && bufList[1] != 0x05)
                            {
                                bufList.Clear();
                                Console.WriteLine("redSwitch port bufList clear");
                                
                            }

                        }

                        if (bufList.Count < 7)
                        {
                            break;
                        }



                        if (bufList[0] == 0x01 && bufList[1] == 0x02) //是查询光耦状态的返回 0x02,正确返回7个字节
                        {
                            if (bufList.Count < 7)
                            {
                                break;
                            }
                            //  Console.WriteLine("check 围栏  ==》 bufList.Count = " + bufList.Count);
                            byte[] data = new byte[7];
                            bufList.CopyTo(0, data, 0, 7);
                            check_optocoupler(data);
                            bufList.RemoveRange(0, 7);
                        }
                        else if (bufList[0] == 0x01 && bufList[1] == 0x05 )//是控制开关闸机的返回 0x05，正确返回8个字节
                        {
                            if (bufList.Count<8)
                            {
                                break;
                            }
                            //  Console.WriteLine(" begin check 开关闸机");
                            byte[] data = new byte[8];
                            bufList.CopyTo(0, data, 0, 8);
                            Check_D01(data);
                            Check_D02(data);
                            bufList.RemoveRange(0, 8);

                        }

                    }
                }
            }
        }

        /// <summary>
        /// 检测围栏遮挡 （optocoupler（光耦））
        /// </summary>
        /// <param name="bytes"></param>
        private void check_optocoupler(byte[] bytes)
        {
            if (bytes[0] == 0x01)//设备地址
            {
                if (bytes[1] == 0x02)//0x02 查询指令，0x82 查询错误
                {

                    if (bytes[2] == 0x02)
                    {
                        chanState[15] = (bytes[4] & 0x80) == 0x80 ? 1 : 0;
                        chanState[14] = (bytes[4] & 0x40) == 0x40 ? 1 : 0;
                        chanState[13] = (bytes[4] & 0x20) == 0x20 ? 1 : 0;
                        chanState[12] = (bytes[4] & 0x10) == 0x10 ? 1 : 0;
                        chanState[11] = (bytes[4] & 0x08) == 0x08 ? 1 : 0;
                        chanState[10] = (bytes[4] & 0x04) == 0x04 ? 1 : 0;
                        chanState[9] = (bytes[4] & 0x02) == 0x02 ? 1 : 0;
                        chanState[8] = (bytes[4] & 0x01) == 0x01 ? 1 : 0;

                        chanState[7] = (bytes[3] & 0x80) == 0x80 ? 1 : 0;
                        chanState[6] = (bytes[3] & 0x40) == 0x40 ? 1 : 0;
                        chanState[5] = (bytes[3] & 0x20) == 0x20 ? 1 : 0;
                        chanState[4] = (bytes[3] & 0x10) == 0x10 ? 1 : 0;
                        chanState[3] = (bytes[3] & 0x08) == 0x08 ? 1 : 0;
                        chanState[2] = (bytes[3] & 0x04) == 0x04 ? 1 : 0;
                        chanState[1] = (bytes[3] & 0x02) == 0x02 ? 1 : 0;
                        chanState[0] = (bytes[3] & 0x01) == 0x01 ? 1 : 0;



                        UpdateRedSwitchUI(chanState[iniChan[0]],
                            chanState[iniChan[1]],
                            chanState[iniChan[2]],
                            chanState[iniChan[3]],
                            chanState[iniChan[4]],
                            chanState[iniChan[5]],
                            chanState[iniChan[6]],
                            chanState[iniChan[7]]); //n5坏了，换成n9
                        // 入厂围栏
                        if (chanState[iniChan[0]] > 0 && chanState[iniChan[1] ]> 0 && chanState[iniChan[2]] > 0 && chanState[iniChan[3]] > 0)
                        {
                            Manager.Instance.m_inRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.Instance.m_inRedSwitchOK = false;
                        }


                        //出厂围栏
                        if (chanState[iniChan[4]] > 0 && chanState[iniChan[5]] > 0 && chanState[iniChan[6]] > 0 && chanState[iniChan[7]] > 0)
                        {
                            Manager.Instance.m_outRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.Instance.m_outRedSwitchOK = false;
                        }
                    }
                }
            }
        }

        private void check_optocoupler_old(byte[] bytes)
        {
            if (bytes[0] == 0x01)//设备地址
            {
                if (bytes[1] == 0x02)//0x02 查询指令，0x82 查询错误
                {

                    if (bytes[2] == 0x02)
                    {

                        n16 = (bytes[4] & 0x80) == 0x80 ? 1 : 0;
                        n15 = (bytes[4] & 0x40) == 0x40 ? 1 : 0;
                        n14 = (bytes[4] & 0x20) == 0x20 ? 1 : 0;
                        n13 = (bytes[4] & 0x10) == 0x10 ? 1 : 0;
                        n12 = (bytes[4] & 0x08) == 0x08 ? 1 : 0;
                        n11 = (bytes[4] & 0x04) == 0x04 ? 1 : 0;
                        n10 = (bytes[4] & 0x02) == 0x02 ? 1 : 0;
                        n9 = (bytes[4] & 0x01) == 0x01 ? 1 : 0;

                        n8 = (bytes[3] & 0x80) == 0x80 ? 1 : 0;
                        n7 = (bytes[3] & 0x40) == 0x40 ? 1 : 0;
                        n6 = (bytes[3] & 0x20) == 0x20 ? 1 : 0;
                        n5 = (bytes[3] & 0x10) == 0x10 ? 1 : 0;
                        n4 = (bytes[3] & 0x08) == 0x08 ? 1 : 0;
                        n3 = (bytes[3] & 0x04) == 0x04 ? 1 : 0;
                        n2 = (bytes[3] & 0x02) == 0x02 ? 1 : 0;
                        n1 = (bytes[3] & 0x01) == 0x01 ? 1 : 0;


                        UpdateRedSwitchUI(n1, n2, n3, n4, n9, n6, n7, n8); //n5坏了，换成n9
                        // 入厂围栏
                        if (n1 > 0 && n2 > 0 && n3 > 0 && n4 > 0)
                        {
                            Manager.Instance.m_inRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.Instance.m_inRedSwitchOK = false;
                        }


                        //出厂围栏
                        if (n9 > 0 && n6 > 0 && n7 > 0 && n8 > 0)
                        {
                            Manager.Instance.m_outRedSwitchOK = true;
                        }
                        else
                        {
                            Manager.Instance.m_outRedSwitchOK = false;
                        }
                    }
                }
            }
        }

        bool AddToQueue(CommdType t,byte[] data)
        {
            foreach (var item in commdQueue)
            {
                if (item.commd_type == t)
                {
                    return false;
                }
            }
            CommandInfo ci = new CommandInfo();
            ci.commd_type = t;
            ci.mData = data;
            commdQueue.Enqueue(ci);
            return true;
        }

        public void RiseIn()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A };
            //  m_serialPort_RedSwitch.WriteTimeout = 1000;
            // m_serialPort_RedSwitch.Write(data, 0, data.Length);

            if (AddToQueue(CommdType.InRise, data))
            {
                Console.WriteLine("Do1 开指令入列");
                checkD01open = true;
            }
           
            //Thread.Sleep(350);
            //DropIn();
        }
        public void DropIn()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x00, 0x00, 0x00, 0xCD, 0xCA };
            if (AddToQueue(CommdType.InDrop, data))
            {
                Console.WriteLine("Do1 关指令入列");
            }
        }
        public void RiseOut()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA };

            if (AddToQueue(CommdType.OutRise, data))
            {
                checkD02open = true;
                Console.WriteLine("Do2 开指令入列");
            }
           
          
        }
        public void DropOut()
        {
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };
            data = AddCRC(data);
            if (AddToQueue(CommdType.OutDrop, data))
            {
                Console.WriteLine("Do2 关指令入列");
            }
            
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
