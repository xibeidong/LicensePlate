using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicensePlate
{
    public enum RootLevel
    {
        Highest,
        Normal,
        Minimum
    }
   public class UserInfo{
        public bool isLogin = false;
        public string username = "admin";

        public string password;
        public RootLevel level = RootLevel.Highest;
    }
    public class Manager
    {
        public delegate void InsertIndexListViewDelegate(string str);
        public InsertIndexListViewDelegate InsertIndexListView;
        public delegate void UpdeteIndexListViewDelegate(string str);
        public UpdeteIndexListViewDelegate UpdeteIndexListView;
        public delegate void ShowMessageDelegate(string message,bool showOrhide);
        public ShowMessageDelegate ShowHideMessage;
        public Func<string, string> GetIDbyInChepai;
        public Action<string> LogToRichText;
        public static Manager instance = new Manager();

        public List<string> m_blackList = new List<string>();//黑名单
        public List<string> m_cheInList = new List<string>();//入厂名单

       // public

        //识别的车牌不重复
        public bool m_inChepaiChange;
        public bool m_outChepaiChange;

        //红外围栏全部无遮挡为true
        public bool m_inRedSwitchOK; 
        public bool m_outRedSwitchOK;

        //忽略红外围栏的作用
        public bool ignore_in_redSwitch = false;
        public bool ignore_out_redSwitch = false;
        public bool isPreviewPrint = false;

        public UserInfo userInfo = new UserInfo();

        //记录当前生成记录的车牌
        public string m_oldInChepai = "123456";
        public string m_oldOutChepai = "123456";

        //记录实时数据
        public string m_inImgPath;
        public string m_outImgPath;
        public string m_inChepai;
        public string m_OutChepai;
        public double m_inWeight;
        public double m_outWeight;

        public LEDControl ledControl;
        public RedSwitch redSwitch;
       
        
       

        private Manager()
        {

        }
        public static Manager Instance
        {
             
            get
            {
                if (instance == null)
                {
                    instance = new Manager();
                }
                return instance;
            }
        }

        public void init()
        {
            MysqlHelp.Instance.init();

        }

        public string CreateInRecord()
        {
            if (Manager.instance.m_inWeight < 1000) //称重低于1吨不处理
            {
                ShowHideMessage("低于1吨，称重异常，请重试！", true);//出厂围栏遮挡的提示隐藏掉
                m_inChepaiChange = false;// 
                ledControl.InLEDTextUpdate("请下磅重试", m_inWeight.ToString());
                return null;
            }

            string str1 = m_cheInList.Find(x => x == m_inChepai);
            if (!string.IsNullOrEmpty(str1))
            {
                Console.WriteLine("已经存在入厂记录:"+m_inChepai);
                ShowHideMessage("禁入！已存在入厂记录:" + m_inChepai, true);
                ledControl.InLEDTextUpdate("请下磅重试", m_inWeight.ToString());
                return null;
                //return "已经存在入厂记录";
            }
            str1 = m_blackList.Find((data)=>data==m_inChepai);
            if (!string.IsNullOrEmpty(str1))
            {
                ShowHideMessage("黑名单成员禁入！:" + m_inChepai, true);
                ledControl.InLEDTextUpdate("已拉黑", m_inWeight.ToString());
                return null;
            }

            //写入数据库
            string t = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string temp_inImgPath = m_inImgPath.Replace("\\", "\\\\");
            string sqlStr = string.Format("insert into chepai (in_time,in_weight,in_chepai,in_img) values ('{3}','{0}','{1}','{2}')", m_inWeight, m_inChepai, temp_inImgPath, t);
            int newId = MysqlHelp.Instance.DoInsert(sqlStr);
            m_oldInChepai = m_inChepai;

            if (m_oldInChepai == m_oldOutChepai)//一辆车出厂后又入厂，设置它可以再出厂
            {
                m_oldOutChepai = "123456";

            }

            m_inChepaiChange = false;
            //显示在listview
            m_cheInList.Add(m_inChepai);//加入入厂名单
            InsertIndexListView(string.Format("{4},{0},,{1},,,{2},{3},", t, m_inWeight, m_inChepai, m_inImgPath,newId));
            System.Threading.Thread.Sleep(3000);//显示3秒车牌
            ledControl.InLEDTextUpdate("请下磅！", m_inWeight.ToString());
            redSwitch.RiseIn();
           // ShowHideMessage(0, false);//入厂围栏遮挡的提示隐藏掉a654321
            return null;
        }
       public void CreateOutRecord()
        {

            if (Manager.instance.m_outWeight < 1000) //称重低于1吨不处理
            {
                ShowHideMessage("低于1吨，称重异常，请重试！", true);
                m_outChepaiChange = false;//提示一次就够了
                ledControl.OutLEDTextUpdate("请下磅重试", m_outWeight.ToString());
                return;
            }
            string temp_outImgPath = m_outImgPath.Replace("\\", "\\\\");
            //更新数据库
            string t = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string IDStr = GetIDbyInChepai(m_OutChepai);
            // string sqlStr = string.Format("update  chepai set out_time='{4}',out_weight='{0}',out_img='{1}',out_chepai='{2}',state=1 where state=0 and in_chepai='{3}' and in_time<'{4}'", m_outWeight, temp_outImgPath, m_OutChepai, m_OutChepai, t);
            string sqlStr = $"update chepai set out_time='{t}',out_weight='{m_outWeight}',out_img='{temp_outImgPath}',state=1 where ID={IDStr}";

            int ret = -1;
            if (IDStr != null)
            {
                ret = MysqlHelp.Instance.Do(sqlStr);
                Log.myLog.Info(sqlStr + "执行结果=" + ret);
            }
           
            //更新数据库成功
            if (ret > 0)
            {
                //更新listview
                //insertToListView1(string.Format(",{0},,{1},,,{2},{3},", t, label_in_weight.Text, strLicense, path));
                UpdeteIndexListView(string.Format("{0},{1},{2},{3}", t, m_outWeight,m_OutChepai, m_outImgPath));
                Log.myLog.Info("updateListView1");
            }
            //更新失败，数据库中没有入厂记录
            else
            {
                //只有出厂记录
                
                sqlStr = string.Format("insert into chepai (out_time,out_weight,out_chepai,out_img,state) values ('{3}','{0}','{1}','{2}',-1)", m_outWeight, m_OutChepai, temp_outImgPath, t);
                int newID = MysqlHelp.Instance.DoInsert(sqlStr);
                // 更新listview  //id，入厂时间，出厂时间，入厂重量，出厂重量，货物重量，车牌号，入厂截图，出厂截图
                InsertIndexListView(string.Format("{4},,{0},,{1},,{2},,{3}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), m_outWeight, m_OutChepai, m_outImgPath,newID));
                Log.myLog.Info("insertToListView1");
            }
            m_oldOutChepai = m_OutChepai;//更新完记录，记录下来经过的上一个车牌
            if (m_oldInChepai== m_oldOutChepai)//一辆车入厂后直接出厂，设置它可以再入厂
            {
                m_oldInChepai = "123456";
                
            }
            m_outChepaiChange = false;

            System.Threading.Thread.Sleep(3000);//显示3秒车牌
            ledControl.OutLEDTextUpdate("请下磅！", m_outWeight.ToString());
            redSwitch.RiseOut();
            m_cheInList.Remove(m_OutChepai);//从入厂车辆名单里面取出
           // ShowHideMessage(1, false);//出厂围栏遮挡的提示隐藏掉
        }

        public bool AddBlackList(string chepai)
        {
            
            if (userInfo.level<RootLevel.Highest)
            {
                ShowHideMessage("用户权限不够", true);
                return false;
            }

            string sqlStr = $"insert into blacklist (chepai,add_time,do_user) values ('{chepai.Trim()}','{System.DateTime.Now}','{userInfo.username}')";
            int ret = MysqlHelp.Instance.DoInsert(sqlStr);
            if (ret<0)
            {
               // ShowHideMessage($"{chepai}加入黑名单失败", true);
                return false;
            }
            else
            {
                m_blackList.Add(chepai);
            }
            return true;
        }

        public bool RemoveBlackList(string chepai)
        {
            string sql = $"delete from blacklist where chepai='{chepai}'";
            int ret = MysqlHelp.Instance.Do(sql);
            if (ret == 1)
            {
                m_blackList.Remove(chepai);
                return true;
            }
            else
            {
                // ShowHideMessage($"{chepai}移除黑名单失败", true);
                return false;
            }
        }
    }
}
