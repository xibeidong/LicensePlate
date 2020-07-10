using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicensePlate
{
    public class CameraLicense
    {
        private VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK m_PlateResultCB1 = null;

        private VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK m_PlateResultCB2 = null;

       

        private string m_sAppPath;

        Dictionary<string, string> chepaiTempDict = new Dictionary<string, string>();

        public delegate void UpdateINChepaiLabelDelegate(string chepai);
        public UpdateINChepaiLabelDelegate UpdateInChepai ;
        public delegate void UpdateOUTChepaiLabelDelegate(string chepai);
        public UpdateOUTChepaiLabelDelegate UpdateOutChepai;

        public delegate void UpdateINChepaiImageDelegate(string imgPath);
        public UpdateINChepaiImageDelegate UpdateInChepaiImage;
        public delegate void UpdateOUTChepaiImageDelegate(string imgPath);
        public UpdateOUTChepaiImageDelegate UpdateOutChepaiImage;

        public void Init()
        {
            VzClientSDK.VzLPRClient_Setup();
            m_sAppPath = System.IO.Directory.GetCurrentDirectory();
            
        }
       
        public void OpenDevice1(IntPtr pictureHand)
        {
            string ip = IniFiles.iniFile.IniReadValue("device1", "ip");
            string com = IniFiles.iniFile.IniReadValue("device1", "com");
            string user = IniFiles.iniFile.IniReadValue("device1", "user");
            string password = IniFiles.iniFile.IniReadValue("device1", "password");
            Log.myLog.Info("ip=" + ip);
            Log.myLog.Info("com=" + com);
            Log.myLog.Info("user=" + user);
            Log.myLog.Info("password=" + password);

            short nPort = Int16.Parse(com);
            int handle = VzClientSDK.VzLPRClient_Open(ip, (ushort)nPort, user, password);
            if (handle == 0)
            {
                MessageBox.Show("入厂车牌识别设备打开失败！");
                Manager.instance.LogToRichText("入厂车牌识别设备打开失败！");
                return;
            }

            //输出到pictureBox_in_video
            VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle, null, IntPtr.Zero, 0);
            int m_nPlayHandle2 = VzClientSDK.VzLPRClient_StartRealPlay(handle, pictureHand);

            // 设置车牌识别结果回调
            m_PlateResultCB1 = new VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK(OnPlateResult1);
            VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle, m_PlateResultCB1, IntPtr.Zero, 1);
        }

        private int OnPlateResult1(int handle, IntPtr pUserData,
                                                IntPtr pResult, uint uNumPlates,
                                                VzClientSDK.VZ_LPRC_RESULT_TYPE eResultType,
                                                IntPtr pImgFull,
                                                IntPtr pImgPlateClip)
        {
            if (eResultType != VzClientSDK.VZ_LPRC_RESULT_TYPE.VZ_LPRC_RESULT_REALTIME)
            {
                VzClientSDK.TH_PlateResult result = (VzClientSDK.TH_PlateResult)Marshal.PtrToStructure(pResult, typeof(VzClientSDK.TH_PlateResult));

                string strLicense = new string(result.license);

               
                //label_in_chepai.Text = strLicense;
                //strLicense = label_in_chepai.Text.Trim();



                strLicense = strLicense.Replace(" ", "");

                Log.myLog.Info(string.Format("chepai='{0}'", strLicense));

                if (Manager.instance.m_oldInChepai == strLicense || strLicense.Contains(Manager.instance.m_oldInChepai))
                {
                    //车牌识别重复，不处理
                    Manager.instance.m_inChepaiChange = false;
                    Console.WriteLine("车牌识别重复，不处理");
                    Console.WriteLine("m_oldInChepai = "+ Manager.instance.m_oldInChepai);
                    Console.WriteLine("strLicense = " + strLicense);

                    MessageBox.Show("车牌和上一车辆相同，禁止入厂，请车辆退出地磅！！");
                    return 0;
                }
                else
                {
                    //车牌
                    Manager.instance.m_inChepaiChange = true;
                }
                Log.myLog.Info("=======>strLicense:  " + strLicense);
                Log.myLog.Info("=======>m_oldInChepai:  " + Manager.instance.m_oldInChepai);

                VzClientSDK.VZ_LPR_MSG_PLATE_INFO plateInfo = new VzClientSDK.VZ_LPR_MSG_PLATE_INFO();
                plateInfo.plate = strLicense;

                DateTime now = DateTime.Now;
                string sTime = string.Format("{0:yyyyMMddHHmmssffff}", now);

                string strFilePath = m_sAppPath + "\\cap_in\\";
                if (!Directory.Exists(strFilePath))
                {
                    Directory.CreateDirectory(strFilePath);
                }

                string path = strFilePath + sTime + ".jpg";

                VzClientSDK.VzLPRClient_ImageSaveToJpeg(pImgFull, path, 100);

                //识别结果显示出来
                //label_in_chepai.Text = strLicense;
                UpdateInChepai(strLicense);

                //pictureBox_in_img.Image = Image.FromFile(path);
                UpdateInChepaiImage(path);

                Manager.instance.ledControl.InLEDTextUpdate(strLicense, "进行中...");

            }

            return 0;
        }

        public void OpenDevice2(IntPtr pictureHand)
        {
            string ip = IniFiles.iniFile.IniReadValue("device2", "ip");
            string com = IniFiles.iniFile.IniReadValue("device2", "com");
            string user = IniFiles.iniFile.IniReadValue("device2", "user");
            string password = IniFiles.iniFile.IniReadValue("device2", "password");
            Log.myLog.Info("ip=" + ip);
            Log.myLog.Info("com=" + com);
            Log.myLog.Info("user=" + user);
            Log.myLog.Info("password=" + password);

            short nPort = Int16.Parse(com);
            int handle = VzClientSDK.VzLPRClient_Open(ip, (ushort)nPort, user, password);
           
            if (handle == 0)
            {
                MessageBox.Show("出厂车牌识别设备打开失败！");
                Manager.instance.LogToRichText("出厂车牌识别设备打开失败！");
                return;
            }

            //输出到pictureBox_out_video
            VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle, null, IntPtr.Zero, 0);
            int m_nPlayHandle2 = VzClientSDK.VzLPRClient_StartRealPlay(handle, pictureHand);

            // 设置车牌识别结果回调
            m_PlateResultCB2 = new VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK(OnPlateResult2);
            VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle, m_PlateResultCB2, IntPtr.Zero, 1);
        }

        private int OnPlateResult2(int handle, IntPtr pUserData,
                                               IntPtr pResult, uint uNumPlates,
                                               VzClientSDK.VZ_LPRC_RESULT_TYPE eResultType,
                                               IntPtr pImgFull,
                                               IntPtr pImgPlateClip)
        {
            if (eResultType != VzClientSDK.VZ_LPRC_RESULT_TYPE.VZ_LPRC_RESULT_REALTIME)
            {
                VzClientSDK.TH_PlateResult result = (VzClientSDK.TH_PlateResult)Marshal.PtrToStructure(pResult, typeof(VzClientSDK.TH_PlateResult));
                string strLicense = new string(result.license);

                //strLicense = strLicense.Replace(" ", "");
                //label_out_chepai.Text = strLicense;
                //strLicense = label_out_chepai.Text.Trim();

                if (Manager.instance.m_oldOutChepai == strLicense || strLicense.Contains(Manager.instance.m_oldOutChepai))
                {
                    Manager.instance.m_outChepaiChange = false;
                    MessageBox.Show("车牌和上一车辆相同，禁止出厂，请车辆退出地磅！！");
                    return 0;
                }
                else
                {
                    Manager.instance.m_outChepaiChange = true;
                }

                Log.myLog.Info(string.Format("chepai='{0}'", strLicense));
                Log.myLog.Info("=======>strLicense:  " + strLicense);
                Log.myLog.Info("=======>m_oldOutChepai:  " + Manager.instance.m_oldOutChepai);

                VzClientSDK.VZ_LPR_MSG_PLATE_INFO plateInfo = new VzClientSDK.VZ_LPR_MSG_PLATE_INFO();
                plateInfo.plate = strLicense;

                DateTime now = DateTime.Now;
                string sTime = string.Format("{0:yyyyMMddHHmmssffff}", now);

                string strFilePath = m_sAppPath + "\\cap_out\\";
                if (!Directory.Exists(strFilePath))
                {
                    Directory.CreateDirectory(strFilePath);
                }

                string path = strFilePath + sTime + ".jpg";

                VzClientSDK.VzLPRClient_ImageSaveToJpeg(pImgFull, path, 100);


                //识别结果显示出来
                // label_out_chepai.Text = strLicense;
                //pictureBox_out_img.Image = Image.FromFile(path);
                UpdateOutChepai(strLicense);
                UpdateOutChepaiImage(path);

                Manager.instance.ledControl.OutLEDTextUpdate(strLicense, "进行中...");

            }

            return 0;
        }

        ~CameraLicense()
        {
            VzClientSDK.VzLPRClient_Cleanup();
            Log.myLog.Info("已关闭车牌识别");
        }

    }
}

/* 1. 识别的车牌转string存在问题，待解决
 
     */