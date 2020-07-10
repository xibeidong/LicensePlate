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
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

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

        private void button5_Click(object sender, EventArgs e)
        {
           string str =  GetSpellCode(textBox1.Text);
            MessageBox.Show(str);
        }


        /// <summary>

        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串

        /// </summary>

        /// <param name="CnStr">汉字字符串</param>

        /// <returns>相对应的汉语拼音首字母串</returns>

        public static string GetSpellCode(string CnStr)
        {

            string strTemp = "";

            int iLen = CnStr.Length;

            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {

                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));

            }

            return strTemp;

        }

        /// <summary>

        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母

        /// </summary>

        /// <param name="CnChar">单个汉字</param>

        /// <returns>单个大写字母</returns>

        private static string GetCharSpellCode(string CnChar)
        {

            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回

            if (ZW.Length == 1)
            {

                return CnChar.ToUpper();

            }

            else
            {

                // get the array of byte from the single char

                int i1 = (short)(ZW[0]);

                int i2 = (short)(ZW[1]);

                iCnChar = i1 * 256 + i2;

            }

            // iCnChar match the constant

            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {

                return "A";

            }

            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {

                return "B";

            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {

                return "C";

            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {

                return "D";

            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {

                return "E";

            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {

                return "F";

            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {

                return "G";

            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {

                return "H";

            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {

                return "J";

            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {

                return "K";

            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {

                return "L";

            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {

                return "M";

            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {

                return "N";

            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {

                return "O";

            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {

                return "P";

            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {

                return "Q";

            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {

                return "R";

            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {

                return "S";

            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {

                return "T";

            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {

                return "W";

            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {

                return "X";

            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {

                return "Y";

            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {

                return "Z";

            }
            else

                return ("?");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int deviceid = 0; deviceid < WaveOut.DeviceCount; deviceid++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceid);
                //capabilities.ProductName;  //ProductName即是声卡名称
            }

            WaveOut waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader("WAV\\smile.mp3");
            waveOutDevice.DeviceNumber = 0;

            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
           
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int deviceid = 0; deviceid < WaveOut.DeviceCount; deviceid++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceid);
                richTextBox1.AppendText($"{deviceid}: {capabilities.ProductName}\r\n");
                //capabilities.ProductName;  //ProductName即是声卡名称
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            WaveOut waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader("WAV\\smile.mp3");
            int id = int.Parse(textBox_ID1.Text);
            waveOutDevice.DeviceNumber = id;

            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            waveOutDevice.PlaybackStopped += (a, b) => {
                waveOutDevice.Dispose();
                audioFileReader.Dispose();
                richTextBox1.AppendText($"{id} 播放完成！\r\n");
            };
            //Thread.Sleep(3000);
            //audioFileReader.Position = 0;
            // waveOutDevice.Play();
            //audioFileReader.Dispose();
            //waveOutDevice.Dispose();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            WaveOut waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader("WAV\\attack.mp3");
            int id = int.Parse(textBox_ID2.Text);
            waveOutDevice.DeviceNumber = id;

            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            waveOutDevice.PlaybackStopped += (a, b) => {
                waveOutDevice.Dispose();
                audioFileReader.Dispose();
                richTextBox1.AppendText($"{id} 播放完成！\r\n");
            };
        }

        private void button10_Click(object sender, EventArgs e)
        {
            test1();
            return;
             string[] strs =  AsioOut.GetDriverNames();
            string asioDriverName = strs[0];
            var asioOut = new AsioOut(asioDriverName);
            
            var outputChannels = asioOut.DriverOutputChannelCount;
            asioOut.InputChannelOffset = 2;
            AudioFileReader mySampleProvider = new AudioFileReader("WAV\\attack.mp3");
           
            asioOut.Init(mySampleProvider);
           
            asioOut.Play();

        }

        void test1()
        {
            var input1 = new Mp3FileReader("WAV\\attack.mp3");
            var input2 = new Mp3FileReader("WAV\\smile.mp3");
            var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1 },2);

            waveProvider.ConnectInputToOutput(0, 0);
            //waveProvider.ConnectInputToOutput(1, 0);
          
            //waveProvider.ConnectInputToOutput(1, 2);
            //waveProvider.ConnectInputToOutput(1, 3);

            string[] strs = AsioOut.GetDriverNames();
            string asioDriverName = strs[0];
            var asioOut = new AsioOut(asioDriverName);

            var outputChannels = asioOut.DriverOutputChannelCount;
           // asioOut.InputChannelOffset = 2;
            //AudioFileReader mySampleProvider = new AudioFileReader("WAV\\attack.mp3");

            asioOut.Init(waveProvider);

            asioOut.Play();

        }

        void testleft()
        {

                var inputReader = new AudioFileReader("WAV\\attack.mp3");
                // convert our stereo ISampleProvider to mono
                var mono = new StereoToMonoSampleProvider(inputReader);
                mono.LeftVolume = 1.0f; // discard the left channel
                mono.RightVolume = 0.0f; // keep the right channel

                // can either use this for playback:
                string[] strs = AsioOut.GetDriverNames();
                string asioDriverName = strs[0];
                var myOutputDevice = new AsioOut(asioDriverName);
                myOutputDevice.Init(mono);
                myOutputDevice.Play();
                // ...

                // ... OR ... could write the mono audio out to a WAV file
               
            
        }

        void testRight()
        {

            var inputReader = new AudioFileReader("WAV\\smile.mp3");
            // convert our stereo ISampleProvider to mono
            var mono = new StereoToMonoSampleProvider(inputReader);
            mono.LeftVolume = 0.0f; // discard the left channel
            mono.RightVolume = 1.0f; // keep the right channel

            // can either use this for playback:
            string[] strs = AsioOut.GetDriverNames();
            string asioDriverName = strs[0];
            var myOutputDevice = new AsioOut(asioDriverName);
            myOutputDevice.Init(mono);
            myOutputDevice.Play();
            // ...

            // ... OR ... could write the mono audio out to a WAV file


        }

    }

   

 }
