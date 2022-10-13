using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BK_Tool
{
    public partial class TDDBReportMain : Form
    {

        // ------普通报告------
        string CaseNo = "";  //病例号码
        string patName = "";  //姓名
        DateTime patBirthday;  //出生年月
        string patSex = "";  //性别
        string address = "";  //地址
        string telephone = "";  //电话

        string patType = ""; //病人类型
        string deptName = ""; //科室
        string roomNo = ""; //房间
        string bedNo = ""; //床号
        string fbType = ""; //费用类型

        string strKey = "";//报告单号
        string sampleNo = "";//样本编号
        DateTime CollectTime;  //采样时间
        DateTime TestTime;  //检验时间
        string Collect_User = ""; //采集者
        string Diagion = ""; //临床诊断信息

        DateTime RecieveTime;//送检时间
        string SampleSource = "";//样本来源  BLDV”-静脉血“BLDC”-末稍血
        DateTime AuditTime;//审核时间
        string Diagnostic = "";//诊断ID 取值为“HM”，意思为 Hematology，即血液学
        string AuditUser = "";//样本审核者

        string strItemNo = "";//项目ID
        string strResult = "";//项目结果
        string strUnits = "";//单位
        string strRange = "";//参考范围
        string strResultflg = "";

        public TDDBReportMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SockectComm sockectComm = new SockectComm();
        }

        #region 对截取到的合格拼接串进行HL7协议处理
        /// <summary>
        /// 对截取到的合格拼接串进行HL7协议处理
        /// </summary>
        /// <param name="ib_data">截取到的合格拼接串</param>
        public void ChuLiOneHL7(string ib_data)
        {
            Common common = new Common();
            bool IsUpdate = false;
            strKey = DateTime.Now.ToString("yyyyMMddhhmmss");//报告单号

            outPutAnaly(strKey, "----解析开始----");
            outPutAnaly("", ib_data);



            Maticsoft.Model.report_main_unaudit report_main_unaudit = new Maticsoft.Model.report_main_unaudit();
            Maticsoft.DAL.report_main_unaudit report_main_unauditDal = new Maticsoft.DAL.report_main_unaudit();
            Maticsoft.Model.report_detail_undudit report_detail1_unaudit = new Maticsoft.Model.report_detail_undudit();
            Maticsoft.DAL.report_detail_undudit report_detailDal_unauditDal = new Maticsoft.DAL.report_detail_undudit();

            Maticsoft.Model.samplemain samplemain = new Maticsoft.Model.samplemain();
            Maticsoft.DAL.samplemain samplemainDal = new Maticsoft.DAL.samplemain();

            report_main_unaudit.KeyNo_Group = strKey;
            report_main_unaudit.REPORT_ID = strKey;
            report_main_unaudit.INSTRUMENT = "TDDB";
            #region 对合格串进行解析
            string[] arrayData = ib_data.Split('\r');
            for (int i = 0; i < arrayData.Length - 1; i++)
            {
                string[] arrayLine = arrayData[i].Split('|');
                switch (arrayLine[0])
                {
                    #region 消息段包含病人的基本信息
                    case "PID":
                        //PID|3731|||||||M|||||||||||||||||1||||||
                        report_main_unaudit.PAT_IN_HOS_ID = arrayLine[2].ToString();
                        report_main_unaudit.PAT_NO = arrayLine[3].ToString();
                        report_main_unaudit.BED = arrayLine[4].ToString();
                        report_main_unaudit.PAT_NAME = arrayLine[5].ToString();
                        if (arrayLine[6].ToString() != "") 
                        {
                            report_main_unaudit.PAT_DEPTName = arrayLine[6].ToString().Split('^')[0];
                            report_main_unaudit.ROOM = arrayLine[6].ToString().Split('^')[1];
                        }
                        report_main_unaudit.PAT_Birthday = common.IsNullCheck(arrayLine[7].ToString()).ToString("G");
                        patSex = arrayLine[8].ToString();//性别
                        if (patSex == "M") { report_main_unaudit.PAT_SEX = "男"; } else if (patSex == "F") { report_main_unaudit.PAT_SEX = "女"; } else { report_main_unaudit.PAT_SEX = "未知"; }
                        report_main_unaudit.BloodType = arrayLine[9].ToString();
                        report_main_unaudit.Address = arrayLine[11].ToString();
                        report_main_unaudit.Telephone = arrayLine[13].ToString();
                        report_main_unaudit.DocMemo = arrayLine[26].ToString();
                        report_main_unaudit.PAT_AGE = common.GetAge(common.IsNullCheck(arrayLine[7].ToString()), DateTime.Now).Split('|')[0];
                        report_main_unaudit.PAT_AGEUnit = common.GetAge(common.IsNullCheck(arrayLine[7].ToString()), DateTime.Now).Split('|')[1];

                        outPutAnaly("患者ID", report_main_unaudit.PAT_IN_HOS_ID);
                        outPutAnaly("床号", report_main_unaudit.BED);
                        outPutAnaly("年龄", report_main_unaudit.PAT_AGE);
                        outPutAnaly("姓名", report_main_unaudit.PAT_NAME);
                        outPutAnaly("生日", report_main_unaudit.PAT_Birthday);
                        outPutAnaly("地址", report_main_unaudit.Address);
                        outPutAnaly("电话", report_main_unaudit.Telephone);
                        outPutAnaly("备注", report_main_unaudit.DocMemo);
                        outPutAnaly("性别", report_main_unaudit.PAT_SEX);                        

                        break;
                    #endregion

                    //#region 包含病人的看病信息。
                    //case "PV1":
                    //    //PV1|1|住院|外科^1^2|||||||||||||||||自费  “科室^房间^床号”
                    //    //patType = arrayLine[2].ToString();//病人类型 => 样本类型
                    //    report_main.SampleType = arrayLine[2].ToString();
                    //    try
                    //    {
                    //        //deptName = arrayLine[3].Split('^')[0];//科室
                    //        //roomNo = arrayLine[3].Split('^')[1];//房间
                    //        //bedNo = arrayLine[3].Split('^')[2];//床号
                    //        bedNo = arrayLine[3].Split('^')[0];//房间
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }

                    //    fbType = arrayLine[20].ToString();//费别

                    //    report_main.PatType = patType;
                    //    report_main.PatDept = deptName;
                    //    report_main.RoomNo = roomNo;
                    //    report_main.BedNo = bedNo;
                    //    report_main.TestName = fbType;

                    //    break;
                    //#endregion

                    #region 仪器信息
                    case "MSH":
                        break;
                    #endregion

                    #region 主要包含检验报告单信息
                    case "OBR":

                        //OBR|3731|Invalid0056|3731|BIOBASE^BK-PA120|N|20221012171000|20220921200943||||||||1|||0||||20221012171000||||||||||||||||||||||||||



                        report_main_unaudit.BARCODE =  arrayLine[2].ToString();
                        report_main_unaudit.SAMPLENO = arrayLine[3].ToString();
                        if (arrayLine[3].ToString() == "Y")
                        {
                            report_main_unaudit.FALG_Emergency = 1;
                        }
                        else { report_main_unaudit.FALG_Emergency = 0; }
                        
                        report_main_unaudit.BarcodeTime = common.IsNullCheck(arrayLine[6].ToString());
                        
                        report_main_unaudit.RegTime =common.IsNullCheck(arrayLine[7].ToString());

                        
                        report_main_unaudit.Diagnosis = arrayLine[13].ToString();

                        report_main_unaudit.RegTime = common.IsNullCheck(arrayLine[14].ToString());
                        if (arrayLine[15].ToString() != "") {
                            if (arrayLine[15].ToString() == "1") { report_main_unaudit.SAMPLEType = "静脉血"; }
                            else if (arrayLine[15].ToString() == "2") { report_main_unaudit.SAMPLEType = "预稀释"; }
                            else if (arrayLine[15].ToString() == "3") { report_main_unaudit.SAMPLEType = "末梢血"; }
                            else if (arrayLine[15].ToString() == "4") { report_main_unaudit.SAMPLEType = "血清"; }
                        }
                                                
                        report_main_unaudit.Send_User = arrayLine[16].ToString();

                        report_main_unaudit.DoctorName = arrayLine[20].ToString();
                        report_main_unaudit.TEST_User = arrayLine[20].ToString();
                        report_main_unaudit.PAT_DEPTName = arrayLine[21].ToString();
                        report_main_unaudit.RptTypeID = 2;
                        report_main_unaudit.REPORT_NAME = "特定蛋白";
                        report_main_unaudit.INSTRUMENT = "TDDB";
                        report_main_unaudit.REG_TYPE = 0;
                        report_main_unaudit.CREATE_DATE = DateTime.Now;
                        report_main_unaudit.OUT_PAT_ID = "-1";


                        if (report_main_unauditDal.ExistsByBarcode(report_main_unaudit.BARCODE))
                        {
                            strKey = report_main_unauditDal.QueryByBarcode(report_main_unaudit.BARCODE).Rows[0]["REPORT_ID"].ToString();
                            report_main_unaudit.REPORT_ID = strKey;
                            report_main_unaudit.KeyNo_Group = strKey;
                            report_main_unauditDal.Update(report_main_unaudit);
                            IsUpdate = true;
                        }
                        else { report_main_unauditDal.Add(report_main_unaudit); }


                        samplemain.BARCODE = arrayLine[2].ToString();
                        samplemain.CREAT_TIME = common.IsNullCheck(arrayLine[6].ToString());
                        samplemain.EXAM_TIME = common.IsNullCheck(arrayLine[7].ToString());
                        samplemain.COLLECT_USER_NAME = arrayLine[10].ToString();
                        samplemain.COLLECT_USER_NAME = arrayLine[16].ToString();
                        samplemain.CREAT_DEPT_NAME = arrayLine[17].ToString();
                        samplemain.EXAM_OPERATOR_NAME = arrayLine[20].ToString();
                        samplemain.RECEIVE_TIME = common.IsNullCheck(arrayLine[14].ToString());
                        samplemain.EXAM_TIME = common.IsNullCheck(arrayLine[14].ToString());


                        #region 样本信息补充
                        samplemain.REPORT_TYPE = "2";
                        samplemain.REPORT_NAME = "特定蛋白";
                        samplemain.REG_TYPE = "0";
                        samplemain.OUT_PAT_ID = "-1";
                        samplemain.PAT_ID = "-1";
                        samplemain.PAT_NAME = report_main_unaudit.PAT_NAME;
                        samplemain.PAT_SEX = report_main_unaudit.PAT_SEX;
                        samplemain.PAT_AGE = report_main_unaudit.PAT_AGE + report_main_unaudit.PAT_AGEUnit;
                        samplemain.ROOM = report_main_unaudit.ROOM;
                        samplemain.BED = report_main_unaudit.BED;
                        samplemain.PAT_NO = report_main_unaudit.PAT_NO;

                        #endregion
                        if (samplemainDal.Exists(samplemain.BARCODE))
                        {
                            samplemainDal.Update(samplemain);
                        }
                        else { samplemainDal.Add(samplemain); }

                        break;
                    #endregion

                    #region 主要包含各个检验结果参数信息
                    case "OBX":

                        //if (arrayLine[2] == "IS")
                        //{
                        //    if (arrayLine[3].IndexOf("Remark") > 0)
                        //    {
                        //        report_main.Demo = arrayLine[5].ToString();
                        //        report_mainDal.Update(report_main);
                        //    }
                        //}
                        //if (arrayLine[3].IndexOf("Histogram") > 0)// 如果循环到这行有Histogram就退出
                        //{                        //当第三位为【ED】，代表图片数据为直方图
                        //    if (arrayLine[2] == "ED")
                        //    {
                        //        //获取图片数据串
                        //        string strImage = arrayLine[5].Split('^')[4];
                        //        if (arrayLine[3].IndexOf("Binary") >= 0)
                        //        {
                        //            //wf_graph(strImage, arrayLine[3].Split('^')[1]);
                        //        }
                        //        else if (arrayLine[3].IndexOf("BMP") >= 0)
                        //        {
                        //            Maticsoft.Model.report_graph report_graph = new Maticsoft.Model.report_graph();
                        //            Maticsoft.DAL.report_graph report_graphDal = new Maticsoft.DAL.report_graph();

                        //            strImage = common.ImgToBase64String(common.GetImgjpg(strImage));
                        //            report_graph.ReportID = strKey;
                        //            report_graph.Graph = strImage;

                        //            if (!IsUpdate) { report_graphDal.Add(report_graph); }

                        //        }
                        //    }
                        //    else { break; }

                        //}

                        //OBX|5105|NM|2|SAA|5.00|mg/L|0.00-10.00|N|||F||5.00|20221012171000||||
                        strItemNo = arrayLine[3].Split('^')[0];

                        //当第三位为【NM】时，代表普通结果
                        if (arrayLine[2] == "NM")
                        {                             
                            strResult = arrayLine[5];
                            strUnits = arrayLine[6];
                            strRange = arrayLine[7];
                            strResultflg = arrayLine[8];


                            #region 参考范围
                            if (strRange.IndexOf("-") >= 0)
                            {
                                string[] strRangeList = strRange.Split('-');
                                if (float.Parse(strRangeList[0]) > float.Parse(strResult))
                                {
                                    strResultflg = "↓";//↓↑
                                }
                                else if (float.Parse(strRangeList[1]) < float.Parse(strResult))
                                {
                                    strResultflg = "↑";//↓↑
                                }
                            }
                            else if (strRange.IndexOf(">") >= 0)
                            {
                                if (float.Parse(strRange) < float.Parse(strResult))
                                {
                                    strResultflg = "↓";//↓↑
                                }
                            }
                            else if (strRange.IndexOf("<") >= 0)
                            {
                                if (float.Parse(strRange) > float.Parse(strResult))
                                {
                                    strResultflg = "↑";//↓↑
                                }
                            }
                            #endregion
                            report_detail1_unaudit.REPORT_ID = strKey;
                            report_detail1_unaudit.KeyNo_Group = strKey;
                            report_detail1_unaudit.ITEM_ID = strItemNo;
                            report_detail1_unaudit.RESULT = strResult;
                            report_detail1_unaudit.UNIT = strUnits;
                            report_detail1_unaudit.REFRANGE = strRange;
                            report_detail1_unaudit.Abnormal_flg = strResultflg;                            
                            report_detail1_unaudit.ITEM_NAME = arrayLine[4]; 
                            report_detail1_unaudit.ITEM_ENAME = arrayLine[4]; 
                            report_detail1_unaudit.RESLT_TIME = common.IsNullCheck(arrayLine[14].ToString());

                            outPutAnaly("报告单号", strKey);
                            outPutAnaly("项目编码", strItemNo);
                            outPutAnaly("项目名称", arrayLine[4]);
                            outPutAnaly("结果", strResult);
                            outPutAnaly("单位", strUnits);
                            outPutAnaly("参考范围", strRange);
                            outPutAnaly("报警表示", strResultflg);


                            if (!IsUpdate) { report_detailDal_unauditDal.Add(report_detail1_unaudit); }
                        }
                        break;
                    #endregion
                    default: break;
                }

            }
            #endregion
        }
        #endregion


        public void outPutAnaly(string key, string log)
        {
            txtAnaly.AppendText(key + ":" + log + "\r\n");
        }

        public void outPut(int flg, string Key, string log)
        {
            if (flg == 0)
            {
                txtLog.AppendText("-----------------开始接收数据-----------------\r\n");
            }
            else
            {
                txtLog.AppendText(Key + ":" + log + "\r\n");
            }

            write(log);
        }

        public void write(string msg)
        {
            //获取当前程序目录
            string logPath = Path.GetDirectoryName(Application.ExecutablePath);
            //新建文件
            System.IO.StreamWriter sw = System.IO.File.AppendText(logPath + "/" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            //写入日志信息
            sw.WriteLine(msg);
            //关闭文件
            sw.Close();
            sw.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = "MSH|^~&|BIOBASE|BK-PA120|||20221012170100||ORU^R01|1580121344501223424|P|2.3.1||||0||ASCII||\r";
            str += "PID|3731|||||||M|||||||||||||||||1||||||\r";
            str += "OBR|3731|Invalid0056|3731|BIOBASE^BK-PA120|N|20221012171000|20220921200943||||||||1|||0||||20221012171000||||||||||||||||||||||||||\r";
            str += "OBX|5104|NM|1|hs-CRP|0.77|mg/L|0.00-3.00|N|||F||0.77|20221012171000||||\r";
            str += "OBX|5105|NM|2|SAA|5.00|mg/L|0.00-10.00|N|||F||5.00|20221012171000||||\r";
            str += "OBX|5106|NM|3|CRP|0.77|mg/L|0.00-10.00|N|||F||0.77|20221012171000||||\r";
            str += "OBX|5107|NM|4|SAA/hs-CRP|1.18|mg/L||N|||F||1.18|20221012171000||||\r";

            ChuLiOneHL7(str);
        }
    }
}
