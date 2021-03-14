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
    public class Loadometer
    {
        public delegate void UpdateInweightDelegate(string str);
        public UpdateInweightDelegate UpdateInweight;
        public delegate void UpdateOutweightDelegate(string str);
        public UpdateOutweightDelegate UpdateOutweight;

        public delegate void UpdateInweightStateDelegate(string str);
        public UpdateInweightStateDelegate UpdateInweightState;
        public delegate void UpdateOutweightStateDelegate(string str);
        public UpdateOutweightStateDelegate UpdateOutweightState;

        public delegate void UpdateInweightConnectDelegate(string str);
        public UpdateInweightConnectDelegate UpdateInweightConnect;
        public delegate void UpdateOutweightConnectDelegate(string str);
        public UpdateOutweightStateDelegate UpdateOutweightConnect;

        SerialPort m_serialPort_in = new SerialPort();
        SerialPort m_serialPort_out = new SerialPort();

        bool inState_ST = false;
        bool outState_ST = false;

        bool isOpenIn = false;
        bool isOpenOut = false;

        System.Timers.Timer loadometer_t1;
        System.Timers.Timer loadometer_t2;

        public void Init()
        {

        }
        ~Loadometer()
        {
            if (loadometer_t1!=null)
            {
                loadometer_t1.Stop();
            }
            if (loadometer_t2 != null)
            {
                loadometer_t2.Stop();
            }
            if (m_serialPort_in.IsOpen)
            {
                m_serialPort_in.Close();
            }
            if (m_serialPort_out.IsOpen)
            {
                m_serialPort_out.Close();
            }
        }
        public void OpenDevice1()
        {
            if (isOpenIn)
            {
                Manager.Instance.LogToRichText("入厂地磅不可以重复打开！");
                return;
            }
            string str_com = IniFiles.iniFile.IniReadValue("weight", "port1");
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
                MessageBox.Show("没有发现串口：" + str_com + " 入厂地磅 open failed!");
                Manager.Instance.LogToRichText("入厂地磅打开失败");
                return;
            }
            m_serialPort_in.PortName = str_com; //"COM6";
            m_serialPort_in.StopBits = StopBits.One;
            m_serialPort_in.Parity = Parity.None;

            m_serialPort_in.BaudRate = 9600;
            m_serialPort_in.DataBits = 8;
            m_serialPort_in.DtrEnable = true;
            m_serialPort_in.RtsEnable = true;
            m_serialPort_in.ReceivedBytesThreshold = 1;
            m_serialPort_in.ReadBufferSize = 4096;
            m_serialPort_in.WriteTimeout = 100000;
            m_serialPort_in.ReadTimeout = 100000;

            try
            {
                m_serialPort_in.Open();
                m_serialPort_in.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived1);

                isOpenIn = true;
                Thread.Sleep(500);
                Log.myLog.Info("串口打开成功:" + str_com);
                UpdateInweightConnect("通讯：OK");
                //label_in_connect.Text = "通讯：OK";
                Manager.Instance.LogToRichText("入厂地磅打开成功");
                Thread.Sleep(500);
                //开起定时器，检测是否生成记录
                System.Timers.Timer tt = new System.Timers.Timer(2000);
                loadometer_t1 = tt;
                tt.Elapsed += new System.Timers.ElapsedEventHandler(timerTheout1);//到达时间的时候执行事件；

                tt.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

                tt.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            }
            catch (Exception e)
            {
                Manager.Instance.LogToRichText("入厂地磅打开失败，" + e.Message);
                ;
            }
           

        }

        private void timerTheout1(object source, System.Timers.ElapsedEventArgs e)
        {

            if (inState_ST)
            {
                if (Manager.Instance.m_inChepaiChange)//识别了新车牌
                {
                   
                    if (Manager.Instance.m_inRedSwitchOK || Manager.Instance.ignore_in_redSwitch)
                    {
                        Manager.Instance.CreateInRecord();
                    }
                    else
                    {
                        Manager.Instance.ShowHideMessage("入厂围栏有遮挡！", true);
                       // MessageBox.Show("入厂围栏有遮挡！");
                    }


                }
            }

          

        }

        private void Serial_DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {

            int intByteCount = m_serialPort_in.BytesToRead;
            byte[] bytes = new byte[intByteCount];
            m_serialPort_in.Read(bytes, 0, intByteCount);
            //strReceiveBuf += Encoding.Default.GetString(bytes);

            // Log.myLog.Info("入厂地磅串口接收长度："+ intByteCount);
            if (intByteCount == 18)
            {
                //ST稳定 US不稳定 OL超载
                string str = Encoding.ASCII.GetString(bytes);//US,GS,+0000050kg CR LF

              //  Log.myLog.Info("串口接收:" + str);
                string[] strs = str.Split(',');
                if (strs[0] == "US")
                {
                    inState_ST = false;
                    UpdateInweightState("称重：不稳定");
                    //label_in_state.Text = "称重：不稳定";
                }
                else if (strs[0] == "ST")
                {
                    //label_in_state.Text = "称重：  稳定";
                    UpdateInweightState("称重： 稳定");
                }
                else if (strs[0] == "OL")
                {
                    inState_ST = false;
                    UpdateInweightState("称重： 超载");
                    //label_in_state.Text = "称重：  超载";
                }

                string w = strs[2];//+0000050kg
                int index = -1;
                if (w.Length >= 8)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (w[i] != '0')
                        {
                            index = i;
                            break;
                        }
                    }
                    string temp_weight;
                    string show_w;
                    if (index==-1)
                    {
                        show_w = "0";
                    }
                    else
                    {
                        show_w = w.Substring(index, 8 - index);
                       
                    }
                    temp_weight = w.Substring(0, 1) + show_w;

                    UpdateInweight(temp_weight);
                    try
                    {
                        Manager.Instance.m_inWeight = double.Parse(temp_weight);
                    }
                    catch (Exception e1)
                    {

                        Log.myLog.Error(temp_weight + "===》string转double时出错："+e1.Message);
                    }
                   
                    if (strs[0] == "ST")
                    {
                        inState_ST = true;
                       // inWeight_ST = 
                        //if (Manager.instance.m_inChepaiChange)//识别了新车牌
                        //{
                        //  //  System.Threading.Thread.Sleep(2000);//停顿2秒，等待围栏状态更新
                        //    if (Manager.instance.m_inRedSwitchOK || Manager.instance.ignore_in_redSwitch)
                        //    {
                        //        Manager.instance.CreateInRecord();
                        //    }
                        //    else
                        //    {
                        //        MessageBox.Show("入厂围栏有遮挡！");
                        //    }


                        //}

                    }

                }
            }

        }

        public void OpenDevice2()
        {
            if (isOpenOut)
            {
                Manager.Instance.LogToRichText("出厂地磅不可以重复打开！");
                return;
            }
            string str_com = IniFiles.iniFile.IniReadValue("weight", "port2");
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
            m_serialPort_out.PortName = str_com; //"COM6";
            m_serialPort_out.StopBits = StopBits.One;
            m_serialPort_out.Parity = Parity.None;

            m_serialPort_out.BaudRate = 9600;
            m_serialPort_out.DataBits = 8;
            m_serialPort_out.DtrEnable = true;
            m_serialPort_out.RtsEnable = true;
            m_serialPort_out.ReceivedBytesThreshold = 1;
            m_serialPort_out.ReadBufferSize = 4096;
            m_serialPort_out.WriteTimeout = 100000;
            m_serialPort_out.ReadTimeout = 100000;

            try
            {
                m_serialPort_out.Open();
                m_serialPort_out.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived2);

                isOpenOut = true;
                Thread.Sleep(500);
                Log.myLog.Info("串口打开成功:" + str_com);
                //label_out_connect.Text = "通讯：OK";
                UpdateOutweightConnect("通讯：OK");
                Manager.Instance.LogToRichText("出厂地磅打开成功");
                Thread.Sleep(500);
                //开起定时器，检测是否生成记录
                System.Timers.Timer tt = new System.Timers.Timer(2000);
                tt.Elapsed += new System.Timers.ElapsedEventHandler(timerTheout2);//到达时间的时候执行事件；

                tt.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

                tt.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

                loadometer_t2 = tt;
            }
            catch (Exception e)
            {
                Manager.Instance.LogToRichText("出厂地磅打开失败，"+e.Message);
                
            }
           
        }

        private void Serial_DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {

            int intByteCount = m_serialPort_out.BytesToRead;
            byte[] bytes = new byte[intByteCount];
            m_serialPort_out.Read(bytes, 0, intByteCount);
            //strReceiveBuf += Encoding.Default.GetString(bytes);
          //  Log.myLog.Info("出厂地磅串口接收长度：" + intByteCount);
            if (intByteCount == 18)
            {
                string str = Encoding.ASCII.GetString(bytes);//US,GS,+0000050kg CR LF
               // Log.myLog.Info("串口接收:" + str);
                string[] strs = str.Split(',');

                if (strs[0] == "US")
                {
                    outState_ST = false;
                    //label_out_state.Text = "称重：不稳定";
                    UpdateOutweightState("称重：不稳定");
                }
                else if (strs[0] == "ST")
                {
                    UpdateOutweightState("称重：  稳定");
                }
                else if (strs[0] == "OL")
                {
                    outState_ST = false;
                    UpdateOutweightState("称重：  超载");
                }

                string w = strs[2];//+0000050kg
                int index = -1;
                if (w.Length >= 8)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (w[i] != '0')
                        {
                            index = i;
                            break;
                        }
                    }

                    string temp_weight;
                    string show_w;
                    if (index == -1)
                    {
                        show_w = "0";
                    }
                    else
                    {
                        show_w = w.Substring(index, 8 - index);

                    }
                    temp_weight = w.Substring(0, 1) + show_w;
                    //label_out_weight.Text 
                    UpdateOutweight(temp_weight);
                    try
                    {
                        Manager.Instance.m_outWeight = double.Parse(temp_weight);
                       
                    }
                    catch (Exception e1)
                    {

                        Log.myLog.Error(temp_weight + "===》string转double时出错：" + e1.Message);
                    }
                    if (strs[0] == "ST")
                    {
                        outState_ST = true;
                        //if (Manager.instance.m_outChepaiChange)//识别了新车牌
                        //{
                        //   // System.Threading.Thread.Sleep(2000);//停顿2秒，等待围栏状态更新

                        //    if (Manager.instance.m_outRedSwitchOK || Manager.instance.ignore_out_redSwitch)
                        //    {
                        //        Manager.instance.CreateOutRecord();
                        //    }
                        //    else
                        //    {
                        //        MessageBox.Show("出厂围栏有遮挡！");
                        //    }

                        //}

                    }

                }
            }

        }

        private void timerTheout2(object source, System.Timers.ElapsedEventArgs e)
        {

           

            if (outState_ST)
            {
                if (Manager.Instance.m_outChepaiChange)//识别了新车牌
                {
                   

                    if (Manager.Instance.m_outRedSwitchOK || Manager.Instance.ignore_out_redSwitch)
                    {
                        Manager.Instance.CreateOutRecord();
                    }
                    else
                    {
                        
                        Manager.Instance.ShowHideMessage("出厂围栏有遮挡！", true);
                        // MessageBox.Show("出厂围栏有遮挡！");
                    }

                }

            }

        }
    }
}
