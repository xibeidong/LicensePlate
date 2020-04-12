using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SerialPortTest
{
    public partial class Form1 : Form
    {

        SerialPort mSerialPort ;
        public Form1()
        {
            InitializeComponent();
            mSerialPort = new SerialPort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  Console.WriteLine(BitConverter.ToString(AddCRC(new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10 })));
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00 };
           // data = AddCRC(data);
            Console.WriteLine(BitConverter.ToString(data));

            string str_com = "COM6";//niFiles.iniFile.IniReadValue("redswitch", "port1");

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
               
                MessageBox.Show("没有发现串口：" + str_com + " open failed!");
                return;
            }
            mSerialPort.PortName = str_com; //"COM6";
            mSerialPort.StopBits = StopBits.One;
            mSerialPort.Parity = Parity.None;

           mSerialPort.BaudRate = 9600;
           mSerialPort.DataBits = 8;
           mSerialPort.DtrEnable = true;
           mSerialPort.RtsEnable = true;
           mSerialPort.ReceivedBytesThreshold = 1;
           mSerialPort.ReadBufferSize = 4096;
           mSerialPort.WriteTimeout = 100000;
           mSerialPort.ReadTimeout = 100000;
           mSerialPort.Open();
           mSerialPort.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
           // mSerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
            Thread.Sleep(500);
           Console.WriteLine("串口打开成功:" + str_com);

            System.Timers.Timer tt = new System.Timers.Timer(1500);
            tt.Elapsed += new System.Timers.ElapsedEventHandler(timerTheoutSendChannel);//到达时间的时候执行事件；

            tt.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；

            tt.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

        }

        private void timerTheoutSendChannel(object source, System.Timers.ElapsedEventArgs e)
        {

            byte[] data = new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10,0x79,0xC6 };
            mSerialPort.Write(data, 0, data.Length);

        }
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (mSerialPort)
            {
                int intByteCount = mSerialPort.BytesToRead;
                byte[] bytes = new byte[intByteCount];

                mSerialPort.Read(bytes, 0, intByteCount);

                Console.WriteLine(BitConverter.ToString(bytes));
            }
        }
        /// <summary>
        /// 开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //D02开： 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA

            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA };
            mSerialPort.Write(data, 0, data.Length);
        }
        /// <summary>
        /// 关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //01-05-00-01-00-00-9C-0A
            byte[] data = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00, 0x9C, 0x0A };
            mSerialPort.Write(data, 0, data.Length);
        }
        /// <summary>
        /// checkState
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            // 0x01, 0x02, 0x00, 0x00, 0x00, 0x10

            byte[] data = new byte[] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x10, 0x79, 0xC6 };
            mSerialPort.Write(data, 0, data.Length);
        }

        
    }
  }
