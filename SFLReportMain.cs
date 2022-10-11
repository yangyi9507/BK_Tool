using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BK_Tool
{
    public partial class SFLReportMain : Form
    {

        // ------普通报告------
        string CaseNo = "";  //病例号码
        string patName = "";  //姓名
        DateTime patBirthday;  //出生年月
        string patSex = "";  //性别

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

        public SFLReportMain()
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

            Maticsoft.Model.report_main report_main = new Maticsoft.Model.report_main();
            Maticsoft.DAL.report_main report_mainDal = new Maticsoft.DAL.report_main();
            Maticsoft.Model.report_detail report_detail1 = new Maticsoft.Model.report_detail();
            Maticsoft.DAL.report_detail report_detailDal = new Maticsoft.DAL.report_detail();
            report_main.ReportID = strKey;

            #region 对合格串进行解析
            string[] arrayData = ib_data.Split('\r');
            for (int i = 0; i < arrayData.Length - 1; i++)
            {
                string[] arrayLine = arrayData[i].Split('|');
                switch (arrayLine[0])
                {
                    #region 消息段包含病人的基本信息
                    case "PID":
                        //PID|1||05012006^^^^MR||^张三||19991001000000|男
                        CaseNo = arrayLine[3].Split('^')[0];//病例号码
                        patName = arrayLine[5].Split('^')[1];//姓名
                        patBirthday = common.IsNullCheck(arrayLine[7].ToString());//出生年月
                        patSex = arrayLine[8].ToString();//性别

                        report_main.PatAge = common.GetAge(patBirthday, DateTime.Now);
                        report_main.Barcode = CaseNo;
                        report_main.PatName = patName;
                        report_main.PatBirthday = patBirthday.ToString("G");
                        if (patSex == "2") { report_main.PatSex = "女"; } else { report_main.PatSex = "男"; }

                        break;
                    #endregion

                    #region 包含病人的看病信息。
                    case "PV1":
                        //PV1|1|住院|外科^1^2|||||||||||||||||自费  “科室^房间^床号”
                        //patType = arrayLine[2].ToString();//病人类型 => 样本类型
                        report_main.SampleType = arrayLine[2].ToString();
                        try
                        {
                            //deptName = arrayLine[3].Split('^')[0];//科室
                            //roomNo = arrayLine[3].Split('^')[1];//房间
                            //bedNo = arrayLine[3].Split('^')[2];//床号
                            bedNo = arrayLine[3].Split('^')[0];//房间
                        }
                        catch (Exception)
                        {
                        }

                        fbType = arrayLine[20].ToString();//费别

                        report_main.PatType = patType;
                        report_main.PatDept = deptName;
                        report_main.RoomNo = roomNo;
                        report_main.BedNo = bedNo;
                        report_main.TestName = fbType;

                        break;
                    #endregion

                    #region 仪器信息
                    case "MSH":
                        break;
                    #endregion

                    #region 主要包含检验报告单信息
                    case "OBR":

                        //OBR|1||6-968|01001^99MRC|||2022-06-15 15:52:41|||李佩||||||||||||||HM||||||||produce\r
                        sampleNo = arrayLine[3].ToString();//样本编号
                        CollectTime = common.IsNullCheck(arrayLine[6].ToString());//采样时间
                        TestTime = common.IsNullCheck(arrayLine[7].ToString());//检验时间
                        report_main.SendDocName = arrayLine[10].ToString();//送检人员                        
                        Diagion = arrayLine[13].ToString();//临床诊断
                        RecieveTime = common.IsNullCheck(arrayLine[14].ToString());//接收时间
                        SampleSource = arrayLine[15].ToString();//样本来源
                        AuditTime = common.IsNullCheck(arrayLine[22].ToString());//审核时间
                        Diagnostic = arrayLine[24].ToString();//诊断ID
                        AuditUser = arrayLine[28].ToString();//审核者
                        report_main.TestDocName = arrayLine[32].ToString();
                        report_main.SampleNo = sampleNo;
                        report_main.Diagnosis = Diagion;
                        if (report_mainDal.ExistsBySampleNo(sampleNo))
                        {
                            strKey = report_mainDal.QueryBySampleNo(sampleNo).Rows[0]["reportID"].ToString();
                            report_main.ReportID = strKey;
                            report_mainDal.Update(report_main);
                            IsUpdate = true;
                        }
                        else { report_mainDal.Add(report_main); }

                        break;
                    #endregion

                    #region 主要包含各个检验结果参数信息
                    case "OBX":
                        //OBX|7|NM|6690-2^WBC^LN||5.51|10*9/L|4.00-10.00||||F
                        //
                        if (arrayLine[2] == "IS")
                        {
                            if (arrayLine[3].IndexOf("Remark") > 0)
                            {
                                report_main.Demo = arrayLine[5].ToString();
                                report_mainDal.Update(report_main);
                            }
                        }
                        if (arrayLine[3].IndexOf("Histogram") > 0)// 如果循环到这行有Histogram就退出
                        {                        //当第三位为【ED】，代表图片数据为直方图
                            if (arrayLine[2] == "ED")
                            {
                                //获取图片数据串
                                string strImage = arrayLine[5].Split('^')[4];
                                if (arrayLine[3].IndexOf("Binary") >= 0)
                                {
                                    //wf_graph(strImage, arrayLine[3].Split('^')[1]);
                                }
                                else if (arrayLine[3].IndexOf("BMP") >= 0)
                                {
                                    Maticsoft.Model.report_graph report_graph = new Maticsoft.Model.report_graph();
                                    Maticsoft.DAL.report_graph report_graphDal = new Maticsoft.DAL.report_graph();

                                    strImage = common.ImgToBase64String(common.GetImgjpg(strImage));
                                    report_graph.ReportID = strKey;
                                    report_graph.Graph = strImage;

                                    if (!IsUpdate) { report_graphDal.Add(report_graph); }

                                }
                            }
                            else { break; }

                        }
                        strItemNo = arrayLine[3].Split('^')[1];

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
                            report_detail1.ReportID = strKey;
                            report_detail1.Result = strResult;
                            report_detail1.Units = strUnits;
                            report_detail1.RefRange = strRange;
                            report_detail1.Abnormal_Flg = strResultflg;
                            report_detail1.ItemNo = strItemNo;
                            report_detail1.ItemName = "";

                            if (!IsUpdate) { report_detailDal.Add(report_detail1); }
                        }
                        break;
                    #endregion
                    default: break;
                }

            }
            #endregion
        }
        #endregion
    }
}
