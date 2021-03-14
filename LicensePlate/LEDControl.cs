using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicensePlate
{
    public class LEDControl
    {

        string ip1 = "192.168.1.199";
        int LedNumber1 = 1;
        int ColorType1 = 3;
        int GrayLevel1 = 1;
        int LedWidth1 = 120;
        int LedHeight1 = 80;
        int LedUse1 = 0;

        string ip2 = "192.168.1.200";
        int LedNumber2 = 1;
        int ColorType2 = 3;
        int GrayLevel2 = 1;
        int LedWidth2 = 120;
        int LedHeight2 = 80;
        int LedUse2 = 0;

        public LEDControl()
        {
            Init();
        }

        ~LEDControl()
        {

        }
        public void Init()
        {
            ip1 = IniFiles.iniFile.IniReadValue("LED1", "ip");

            string LedNumberStr = IniFiles.iniFile.IniReadValue("LED1", "LedNumber");
            LedNumber1 = int.Parse(LedNumberStr);
            
            string ColorTypeStr = IniFiles.iniFile.IniReadValue("LED1", "ColorType");
            ColorType1 = int.Parse(ColorTypeStr);

            string GrayLevelStr = IniFiles.iniFile.IniReadValue("LED1", "GrayLevel");
            GrayLevel1 = int.Parse(GrayLevelStr);

            string LedWidthStr = IniFiles.iniFile.IniReadValue("LED1", "LedWidth");
            LedWidth1 = int.Parse(LedWidthStr);

            string LedHeightStr = IniFiles.iniFile.IniReadValue("LED1", "LedHeight");
            LedHeight1 = int.Parse(LedHeightStr);

            string Leduse1Str = IniFiles.iniFile.IniReadValue("LED1", "use");
            LedUse1 = int.Parse(Leduse1Str);


            ip2 = IniFiles.iniFile.IniReadValue("LED2", "ip");

            LedNumberStr = IniFiles.iniFile.IniReadValue("LED2", "LedNumber");
            LedNumber2 = int.Parse(LedNumberStr);

             ColorTypeStr = IniFiles.iniFile.IniReadValue("LED2", "ColorType");
            ColorType2 = int.Parse(ColorTypeStr);

            GrayLevelStr = IniFiles.iniFile.IniReadValue("LED2", "GrayLevel");
            GrayLevel2 = int.Parse(GrayLevelStr);

            LedWidthStr = IniFiles.iniFile.IniReadValue("LED2", "LedWidth");
            LedWidth2 = int.Parse(LedWidthStr);

            LedHeightStr = IniFiles.iniFile.IniReadValue("LED2", "LedHeight");
            LedHeight2 = int.Parse(LedHeightStr);

            string Leduse2Str = IniFiles.iniFile.IniReadValue("LED2", "use");
            LedUse2 = int.Parse(Leduse2Str);

        }
        public void OpenDevice1()
        {
            if (LedUse1==0)
            {
                return;//不使用LED功能
            }
            
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                        //ZeroMemory(&CommunicationInfo,sizeof(COMMUNICATIONINFO));
                                                                                        //TCP通讯********************************************************************************
            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip1;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值


            
            nResult = LedDll.LV_SetBasicInfoEx(ref CommunicationInfo, ColorType1, GrayLevel1, LedWidth1, LedHeight1);//设置屏参，屏的颜色为2即为双基色，64为屏宽点数，32为屏高点数，具体函数参数说明见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Log.myLog.Info("入厂LED，设置失败！");
                MessageBox.Show(ErrStr);
                Manager.Instance.LogToRichText("入厂LED，"+ErrStr);
                return;
            }
            else
            {
                Log.myLog.Info("入厂LED，设置成功");
               // MessageBox.Show("设置成功");
            }

           

           // Console.WriteLine(LedWidth1+"*"+ LedHeight1);

        }

        public void OpenDevice2()
        {
            if (LedUse2 == 0)
            {
                return;//不使用LED功能
            }
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                        //ZeroMemory(&CommunicationInfo,sizeof(COMMUNICATIONINFO));
                                                                                        //TCP通讯********************************************************************************
            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip2;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber2;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值



            nResult = LedDll.LV_SetBasicInfoEx(ref CommunicationInfo, ColorType2, GrayLevel2, LedWidth2, LedHeight2);//设置屏参，屏的颜色为2即为双基色，64为屏宽点数，32为屏高点数，具体函数参数说明见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Log.myLog.Info("出厂LED，设置失败！");
                MessageBox.Show(ErrStr);
                Manager.Instance.LogToRichText("出厂LED，" + ErrStr);
                return;
            }
            else
            {
                Log.myLog.Info("出厂LED，设置成功");
                // MessageBox.Show("设置成功");
            }



            //Console.WriteLine(LedWidth2 + "*" + LedHeight2);
        }

       public void AdjustTime()
        {
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                        //ZeroMemory(&CommunicationInfo,sizeof(COMMUNICATIONINFO));
                                                                                        //TCP通讯********************************************************************************
            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip1;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值

            nResult = LedDll.LV_AdjustTime(ref CommunicationInfo);
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Log.myLog.Info("LED1 校时失败！");
                MessageBox.Show("LED1 校时失败! err = "+ErrStr);
            }
            else
            {
                Log.myLog.Info("LED1 校时成功");
            }


            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip2;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber2;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值

            nResult = LedDll.LV_AdjustTime(ref CommunicationInfo);
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Log.myLog.Info("LED2 校时失败！");
                MessageBox.Show("LED2 校时失败! err = " + ErrStr);
            }
            else
            {
                Log.myLog.Info("LED2 校时成功");
            }
        }
        public void TestLEDTextUpdate(string chepai,string weight)
        {

            if (chepai==null&&weight==null)
            {
                Console.WriteLine("LED1 clear");
            }
            else
            {
                double w;
                if (double.TryParse(weight, out w))
                {
                    w = w / 1000;
                }
                else
                {
                    w = 0;
                }
                if (w == 0)
                {
                    weight = "称重中...";
                }
                else
                {
                    weight = "称重:" + w + "吨";
                }
            }
           
           
           
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                        //ZeroMemory(&CommunicationInfo,sizeof(COMMUNICATIONINFO));
                                                                                        //TCP通讯********************************************************************************

            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip1;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值


            int hProgram = 0;//节目句柄
            hProgram = LedDll.LV_CreateProgramEx(LedWidth1, LedHeight1, ColorType1, GrayLevel1, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

            nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
          
            #region chepai
            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 25;
            AreaRect.width = 120;
            AreaRect.height = 25;

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_YELLOW;
            FontProp.FontBold = 0;
            //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, chepai, ref FontProp, 1, 2, 1);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //
#endregion
            #region weight
            AreaRect.left = 0;
            AreaRect.top = 50;
            AreaRect.width = 120;
            AreaRect.height = 30;


            FontProp.FontName = "宋体";
            FontProp.FontSize = 10;
            FontProp.FontColor = LedDll.COLOR_YELLOW;
            FontProp.FontBold = 0;
            //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 2, ref AreaRect, 0);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 2, LedDll.ADDTYPE_STRING, weight, ref FontProp, 1, 2, 1);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }

            #endregion
            //
            #region 日期
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = 120;
            AreaRect.height = 25;
            LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
            DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;
            DigitalClockAreaInfo.DateColor = LedDll.COLOR_RED;
            DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
            DigitalClockAreaInfo.ShowStrFont.FontSize = 8;
            // DigitalClockAreaInfo.ShowStr = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

            DigitalClockAreaInfo.IsShowYear = 1;
            DigitalClockAreaInfo.IsShowMonth = 1;
            DigitalClockAreaInfo.IsShowDay = 1;
            DigitalClockAreaInfo.IsShowHour = 1;
            DigitalClockAreaInfo.IsShowMinute = 1;
           // DigitalClockAreaInfo.IsShowSecond = 1;
            DigitalClockAreaInfo.DateFormat = 4;
            DigitalClockAreaInfo.TimeFormat = 2;

            nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 3, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示


            //AreaRect.left = 0;
            //AreaRect.top = 0;
            //AreaRect.width = 120;
            //AreaRect.height = 25;


            //FontProp.FontName = "宋体";
            //FontProp.FontSize = 10;
            //FontProp.FontColor = LedDll.COLOR_RED;
            //FontProp.FontBold = 0;
            ////int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            //// nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            //// nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            //nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 3, ref AreaRect, 0);
            //if (nResult != 0)
            //{
            //    string ErrStr;
            //    ErrStr = LedDll.LS_GetError(nResult);
            //    Console.WriteLine(ErrStr);
            //    return;
            //}
            //string t = DateTime.Now.ToString("D");
            ////nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            //nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 3, LedDll.ADDTYPE_STRING, t, ref FontProp, 1, 2, 1);
            //if (nResult != 0)
            //{
            //    string ErrStr;
            //    ErrStr = LedDll.LS_GetError(nResult);
            //    Console.WriteLine(ErrStr);
            //    return;
            //}


            #endregion

            nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
            }
            else
            {
                Console.WriteLine("发送成功");
            }
        }
        public void TestLEDTextUpdate2(string chepai, string weight)
        {

            if (chepai == null && weight == null)
            {
                Console.WriteLine("LED2 clear");
            }
            else
            {
                double w;
                if (double.TryParse(weight, out w))
                {
                    w = w / 1000;
                }
                else
                {
                    w = 0;
                }

                if (w == 0)
                {
                    weight = "称重中...";
                }
                else
                {
                    weight = "称重:" + w + "吨";
                }
            }


            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                        //ZeroMemory(&CommunicationInfo,sizeof(COMMUNICATIONINFO));
                                                                                        //TCP通讯********************************************************************************

            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = ip2;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = LedNumber2;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值


            int hProgram = 0;//节目句柄
            hProgram = LedDll.LV_CreateProgramEx(LedWidth2, LedHeight2, ColorType2, GrayLevel2, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败
      
            nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }

            #region chepai
            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 25;
            AreaRect.width = 120;
            AreaRect.height = 25;

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_YELLOW;
            FontProp.FontBold = 0;
            //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, chepai, ref FontProp, 1, 2, 1);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //
            #endregion
            #region weight
            AreaRect.left = 0;
            AreaRect.top = 50;
            AreaRect.width = 120;
            AreaRect.height = 30;


            FontProp.FontName = "宋体";
            FontProp.FontSize = 10;
            FontProp.FontColor = LedDll.COLOR_YELLOW;
            FontProp.FontBold = 0;
            //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 2, ref AreaRect, 0);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }
            //nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 2, LedDll.ADDTYPE_STRING, weight, ref FontProp, 1, 2, 1);
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
                return;
            }

            #endregion
            //
            #region 日期
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = 120;
            AreaRect.height = 25;
            LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
            DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;
            DigitalClockAreaInfo.DateColor = LedDll.COLOR_RED;
            DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
            DigitalClockAreaInfo.ShowStrFont.FontSize = 8;
            // DigitalClockAreaInfo.ShowStr = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

            DigitalClockAreaInfo.IsShowYear = 1;
            DigitalClockAreaInfo.IsShowMonth = 1;
            DigitalClockAreaInfo.IsShowDay = 1;
            DigitalClockAreaInfo.IsShowHour = 1;
            DigitalClockAreaInfo.IsShowMinute = 1;
            // DigitalClockAreaInfo.IsShowSecond = 1;
            DigitalClockAreaInfo.DateFormat = 4;
            DigitalClockAreaInfo.TimeFormat = 2;

            nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 3, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示


            //AreaRect.left = 0;
            //AreaRect.top = 0;
            //AreaRect.width = 120;
            //AreaRect.height = 25;


            //FontProp.FontName = "宋体";
            //FontProp.FontSize = 10;
            //FontProp.FontColor = LedDll.COLOR_RED;
            //FontProp.FontBold = 0;
            ////int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            //// nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
            //// nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
            //nResult = LedDll.LV_AddImageTextArea(hProgram, 1, 3, ref AreaRect, 0);
            //if (nResult != 0)
            //{
            //    string ErrStr;
            //    ErrStr = LedDll.LS_GetError(nResult);
            //    Console.WriteLine(ErrStr);
            //    return;
            //}
            //string t = DateTime.Now.ToString("D");
            ////nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示
            //nResult = LedDll.LV_AddStaticTextToImageTextArea(hProgram, 1, 3, LedDll.ADDTYPE_STRING, t, ref FontProp, 1, 2, 1);
            //if (nResult != 0)
            //{
            //    string ErrStr;
            //    ErrStr = LedDll.LS_GetError(nResult);
            //    Console.WriteLine(ErrStr);
            //    return;
            //}


            #endregion

            nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                Console.WriteLine(ErrStr);
            }
            else
            {
                Console.WriteLine("发送成功");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chepai">车牌号 or 称重完毕请下磅</param>
        /// <param name="weight"></param>

        public void InLEDTextUpdate(string chepai,string weight)
        {
            if (LedUse1 == 0)
            {
                return;//不使用LED功能
            }

            TestLEDTextUpdate(chepai, weight);
           

           
            //定时清屏
            if (chepai.Contains("请下")||chepai.Contains("已拉黑")||chepai.Contains("禁"))
            {
                System.Timers.Timer t = new System.Timers.Timer(15000);//实例化Timer类，设置间隔时间为10000毫秒；

                t.Elapsed += new System.Timers.ElapsedEventHandler(theoutClearLED1);//到达时间的时候执行事件；

                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；

                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            }
        }

        public void OutLEDTextUpdate(string chepai, string weight)
        {
            if (LedUse2 == 0)
            {
                return;//不使用LED功能
            }

            TestLEDTextUpdate2(chepai, weight);



            //出现"请下磅"之后，定时清屏
            if (chepai.Contains("请下") || chepai.Contains("已拉黑") || chepai.Contains("禁"))
            {
                System.Timers.Timer t = new System.Timers.Timer(15000);//实例化Timer类，设置间隔时间为10000毫秒；

                t.Elapsed += new System.Timers.ElapsedEventHandler(theoutClearLED2);//到达时间的时候执行事件；

                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；

                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            }

        }


        public void theoutClearLED1(object source, System.Timers.ElapsedEventArgs e)
        {

            InLEDTextUpdate(null, null);

        }

        public void theoutClearLED2(object source, System.Timers.ElapsedEventArgs e)
        {

            OutLEDTextUpdate(null, null);

        }
    }
}
