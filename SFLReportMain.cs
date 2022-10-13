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

            outPutAnaly(strKey, "----解析开始----");

            Maticsoft.Model.report_main_unaudit report_main_unaudit = new Maticsoft.Model.report_main_unaudit();
            Maticsoft.DAL.report_main_unaudit report_main_unauditDal = new Maticsoft.DAL.report_main_unaudit();
            Maticsoft.Model.report_detail_undudit report_detail1_unaudit = new Maticsoft.Model.report_detail_undudit();
            Maticsoft.DAL.report_detail_undudit report_detailDal_unaudit = new Maticsoft.DAL.report_detail_undudit();
            report_main_unaudit.KeyNo_Group = strKey;
            report_main_unaudit.REPORT_ID = strKey;
            report_main_unaudit.INSTRUMENT = "SFL";
            #region 对合格串进行解析
            string[] arrayData = ib_data.Split('\r');
            for (int i = 0; i < arrayData.Length - 1; i++)
            {
                string[] arrayLine = arrayData[i].Split('|');
                switch (arrayLine[0])
                {
                    #region 消息段包含病人的基本信息
                    case "PID":
                        outPut(1, "PID", arrayData[i].ToString());
                        //PID|1||05012006^^^^MR||^张三||19991001000000|男
                        CaseNo = arrayLine[3].Split('^')[0];//病例号码
                        patName = arrayLine[5].Split('^')[1];//姓名
                        patBirthday = common.IsNullCheck(arrayLine[7].ToString());//出生年月
                        patSex = arrayLine[8].ToString();//性别
                        
                        report_main_unaudit.PAT_AGE = common.GetAge(patBirthday, DateTime.Now).Split('|')[0];
                        report_main_unaudit.PAT_AGEUnit = common.GetAge(patBirthday, DateTime.Now).Split('|')[1];
                        report_main_unaudit.PAT_NO = CaseNo;
                        report_main_unaudit.PAT_NAME = patName;
                        report_main_unaudit.PAT_Birthday = patBirthday.ToString("G");
                        if (patSex == "2") { report_main_unaudit.PAT_SEX = "女"; } else { report_main_unaudit.PAT_SEX = "男"; }

                        outPutAnaly("病历号", report_main_unaudit.PAT_NO);
                        outPutAnaly("姓名", report_main_unaudit.PAT_NAME);
                        outPutAnaly("性别", report_main_unaudit.PAT_SEX);
                        outPutAnaly("生日", report_main_unaudit.PAT_Birthday);


                        break;
                    #endregion

                    #region 包含病人的看病信息。
                    case "PV1":
                        //PV1|1|住院|外科^1^2|||||||||||||||||自费  “科室^房间^床号”
                        //patType = arrayLine[2].ToString();//病人类型 => 样本类型

                        outPut(1, "PV1", arrayData[i].ToString());
                        report_main_unaudit.SAMPLEType = arrayLine[2].ToString();
                        try
                        {
                            //deptName = arrayLine[3].Split('^')[0];//科室
                            //roomNo = arrayLine[3].Split('^')[1];//房间
                            //bedNo = arrayLine[3].Split('^')[2];//床号
                            report_main_unaudit.BED = arrayLine[3].Split('^')[0];//房间
                        }
                        catch (Exception)
                        {
                        }

                        deptName = arrayLine[20].ToString();//科室

                        //report_main_unaudit. = patType;
                        report_main_unaudit.PAT_DEPTName = deptName;
                        //report_main_unaudit.RoomNo = roomNo;
                        //report_main_unaudit.BedNo = bedNo;
                        //report_main_unaudit.TestName = fbType;

                        break;
                    #endregion

                    #region 仪器信息
                    case "MSH":
                        outPut(1, "MSH", arrayData[i].ToString());
                        break;
                    #endregion

                    #region 主要包含检验报告单信息
                    case "OBR":
                        outPut(1, "OBR", arrayData[i].ToString());

                        //OBR|1||6-968|01001^99MRC|||2022-06-15 15:52:41|||李佩||||||||||||||HM||||||||produce\r
                        sampleNo = arrayLine[3].ToString();//样本编号
                        CollectTime = common.IsNullCheck(arrayLine[6].ToString());//采样时间
                        TestTime = common.IsNullCheck(arrayLine[7].ToString());//检验时间
                        report_main_unaudit.Send_User = arrayLine[10].ToString();//送检人员                        
                        Diagion = arrayLine[13].ToString();//临床诊断
                        RecieveTime = common.IsNullCheck(arrayLine[14].ToString());//接收时间
                        SampleSource = arrayLine[15].ToString();//样本来源
                        AuditTime = common.IsNullCheck(arrayLine[22].ToString());//审核时间
                        Diagnostic = arrayLine[24].ToString();//诊断ID
                        AuditUser = arrayLine[28].ToString();//审核者
                        report_main_unaudit.TEST_User = arrayLine[32].ToString();
                        report_main_unaudit.SAMPLENO = sampleNo;
                        report_main_unaudit.Diagnosis = Diagion;

                        outPutAnaly("送检医生", report_main_unaudit.Send_User);
                        outPutAnaly("检验医生", report_main_unaudit.TEST_User);
                        outPutAnaly("样本号码", report_main_unaudit.SAMPLENO);
                        outPutAnaly("诊断信息", report_main_unaudit.Diagnosis);


                        #region 信息补充

                        report_main_unaudit.RptTypeID = 1;
                        report_main_unaudit.REPORT_NAME = "生化";
                        report_main_unaudit.BARCODE = "01" + DateTime.Now.ToString("yyyymmddhhmmss");
                        report_main_unaudit.REG_TYPE =0;
                        report_main_unaudit.CREATE_DATE = DateTime.Now;
                        report_main_unaudit.BarcodeTime = DateTime.Now;
                        report_main_unaudit.RegTime = DateTime.Now;
                        report_main_unaudit.OUT_PAT_ID = "-1";
                        report_main_unaudit.TEST_DATE = DateTime.Now;

                        #endregion

                        if (report_main_unauditDal.ExistsBySampleNo(sampleNo))
                        {
                            strKey = report_main_unauditDal.QueryBySampleNo(sampleNo).Rows[0]["REPORT_ID"].ToString();
                            report_main_unaudit.REPORT_ID = strKey;
                            report_main_unaudit.KeyNo_Group = strKey;
                            report_main_unauditDal.Update(report_main_unaudit);
                            IsUpdate = true;
                        }
                        else { report_main_unauditDal.Add(report_main_unaudit); }

                        break;
                    #endregion

                    #region 主要包含各个检验结果参数信息
                    case "OBX":
                        outPut(1, "OBX", arrayData[i].ToString());
                        //OBX|7|NM|6690-2^WBC^LN||5.51|10*9/L|4.00-10.00||||F
                        //
                        if (arrayLine[2] == "IS")
                        {
                            if (arrayLine[3].IndexOf("Remark") > 0)
                            {
                                report_main_unaudit.DocMemo = arrayLine[5].ToString();
                                report_main_unauditDal.Update(report_main_unaudit);
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
                            report_detail1_unaudit.REPORT_ID = strKey;
                            report_detail1_unaudit.KeyNo_Group = strKey;
                            report_detail1_unaudit.RESULT = strResult;
                            report_detail1_unaudit.UNIT = strUnits;
                            report_detail1_unaudit.REFRANGE = strRange;
                            report_detail1_unaudit.Abnormal_flg = strResultflg;
                            report_detail1_unaudit.ITEM_ID = strItemNo;
                            report_detail1_unaudit.ITEM_NAME = "";
                            report_detail1_unaudit.ITEM_ENAME = "";

                            outPutAnaly("报告单号", strKey);
                            outPutAnaly("项目编码", strItemNo);
                            outPutAnaly("结果", strResult);
                            outPutAnaly("单位", strUnits);
                            outPutAnaly("参考范围", strRange);
                            outPutAnaly("报警表示", strResultflg);

                            if (!IsUpdate) { report_detailDal_unaudit.Add(report_detail1_unaudit); }
                        }
                        break;
                    #endregion
                    default: break;
                }

            }
            #endregion
        }
        #endregion
        public void outPutAnaly(string key , string log)
        {
            this.Invoke(new Action(() =>
            {
                txtAnaly.AppendText(key + ":" + log + "\r\n");
            }));                  
        }

        public void outPut(int flg, string Key ,string log)
        {
            if (flg == 0)
            {                
                this.Invoke(new Action(() =>
                {
                    txtLog.AppendText("-----------------开始接收数据-----------------\r\n");
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    txtLog.AppendText(Key + ":" + log + "\r\n");
                }));                
            }
            write(log);
        }

        public void write(string msg) 
        {
            //获取当前程序目录
            string logPath = Path.GetDirectoryName(Application.ExecutablePath);
            //新建文件
            System.IO.StreamWriter sw = System.IO.File.AppendText(logPath + "/" + DateTime.Now.ToString("yyyyMMdd") +".txt");
            //写入日志信息
            sw.WriteLine(msg);
            //关闭文件
            sw.Close();
            sw.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = "MSH|^~&|HEMA5X|Hema|||20000102024448||ORU^R01|d1ae75d7-8cd1-4d93|P|2.3.1||||||UNICODE\r";
            str += "PID|1||10001^^^^MR||^杨桑||19950725000000|1\r";
            str += "PV1|1|静脉血|2#|||||||||||||||||内科\r";
            str += "OBR|1||6-968|01001^99MRC|||2022-06-15 15:52:41|||老师||||||||||||||HM||||||||produce\r";
            str += "OBX|1|IS|08001^Take Mode^99MRC||O||||||F\r";
            str += "OBX|2|IS|08002^Blood Mode^99MRC||W||||||F\r";
            str += "OBX|3|IS|08003^Test Mode^99MRC||CBC+DIFF||||||F\r";
            str += "OBX|4|NM|30525-0^Age^LN|||0|||||F\r";
            str += "OBX|5|IS|01001^Remark^99MRC||不能喝酒||||||F\r";
            str += "OBX|6|IS|01002^Ref Group^99MRC||0||||||F\r";
            str += "OBX|7|NM|6690-2^WBC^LN||9.63|10^9/L|4-10||||F\r";
            str += "OBX|8|NM|770-8^LYM%^LN||23.9|%|20-50||||F\r";
            str += "OBX|9|NM|736-9^MID%^LN||7.9|%|3-10||||F\r";
            str += "OBX|10|NM|5905-5^GRA%^LN||68.2|%|40-70||||F\r";
            str += "OBX|13|NM|751-8^LYM#^LN||2.3|10^9/L|0.8-4||||F\r";
            str += "OBX|14|NM|731-0^MID#^LN||0.76|10^9/L|0.1-0.9||||F\r";
            str += "OBX|15|NM|742-7^GRA#^LN||6.57|10^9/L|2-7||||F\r";
            str += "OBX|22|NM|789-8^RBC^LN||4.61|10^12/L|3.5-5.5||||F\r";
            str += "OBX|23|NM|718-7^HGB^LN||133|g/L|110-160||||F\r";
            str += "OBX|24|NM|4544-3^HCT^LN||43.1|%|37-54||||F\r";
            str += "OBX|25|NM|787-2^MCV^LN||93.4|fL|80-100||||F\r";
            str += "OBX|26|NM|785-6^MCH^LN||28.8|pg|27-34||||F\r";
            str += "OBX|27|NM|786-4^MCHC^LN||309|g/L|320-360||||F\r";
            str += "OBX|28|NM|788-0^RDW-CV^LN||12.4|%|11-16||||F\r";
            str += "OBX|29|NM|21000-5^RDW-SD^LN||49.6|fL|35-56||||F\r";
            str += "OBX|30|NM|777-3^PLT^LN||266|10^9/L|100-300||||F\r";
            str += "OBX|31|NM|32623-1^MPV^LN||7.7|fL|6.5-12||||F\r";
            str += "OBX|32|NM|32207-3^PDW^LN||16.9|%|9-17||||F\r";
            str += "OBX|33|NM|11003^PCT^99MRC||0.203|%|0.108-0.282||||F\r";
            str += "OBX|34|NM|48386-7^P-LCR^ LN||71|10^9/L|30-90||||F\r";
            str += "OBX|35|NM|34167-7^P-LCC^LN||26.6|%|11-45||||F\r";
            str += "OBX|50|IS|12104^SampleCount Histogram GraphData^99MRC||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4gDpAO8A9AD4APsA/QD+AP8A/gD9APoA9wDyAO0A6ADiANsA1ADNAMcAwQC8ALkAtgCzALEAsACvAK0AqwCpAKYAowCgAJ4AmwCYAJUAkgCQAI0AigCHAIMAgAB9AHkAdwB0AHIAcQBvAG0AawBpAGcAZABhAF0AWgBWAFMAUABNAEsASQBIAEcARQBEAEQAQgBBAEAAPgA9ADsAOgA5ADgANwA2ADYANQA0ADQAMwAyADEAMAAvAC4ALQAsACoAKAAlACMAIAAdABoAFwAVABIAEAAOAAwACwAKAAoACgAKAAoACgAKAAoACgAKAAoACgALAAsACwALAAsACwALAAsACgAKAAoACQAJAAkACQAJAAgACAAIAAgACAAIAAgABwAHAAcABwAHAAYABgAGAAYABgAGAAYABgAGAAcABwAHAAcABwAHAAYABgAGAAYABgAGAAUABQAFAAUABAAEAAUABQAFAAYABgAHAAgACAAJAAoACgAKAAsACwAKAAoACgAJAAgABwAGAAUACgDPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAnAFAAXABmAJIAiQCcAIYAawBQAFQAPAA1ACYAPAAkADsANAA6ACsAHAAmABwAIgAoACIAIgAkABgAJAAkACIAJgAyACIAJgAcACwAMAAaACIAGgAeABwAFwAsACgAIgAUACAAGgAiABgAKAAcACYAKgAgACYAHgAmAB4AIgAiACoAFgAkACsAJgAuADQAKgA0ADcAQwBCADwATABgADoARgBOAEYATQBWAE4AWABZAGAAbABgAFgAUAByAFQAcwB8AEYAbABqAFIAaQBeAFoAbABWAFsAbABLAGgAWgBUAGIAcgBMAGYAWABaAFgAUgBYAD4AQgA2AEwASABEADkAMgAyACYAPgA6ACYAIgAcACoALgAeAC4AEgAcABwAGgAeABwAGAAQABQAEAAYAAgADgAIAAYACgAGAAgADAAIAAYABgAOAAAABgAGAAYABgACAAgABAAEAAQAAAACAAIABAAEAAIABAACAAAAAgAAAAIAAAAAAAIAAAACAAQAAAAAAAAABAAAAAAAAAACAAAAAAAAAAIAAAAAAAAAAAACAAQAAAAAAAAAAAACAAAAAAAAAAIAAgAAAAIAAgAAAAAAAAAAAAIAAAAAAAIAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABIAFAAKAAQACAAGAAgAAAAGAAQACAACAAIACAAEAAgACAAAAAQABAAEAAIABgAGAAoACgAMACQAEgAYADUAQgBoAH4AqwC/AO4APAF5AesBPgJ3AvgCcQOgA8ADWwQNBNQEkQR0BNkEtAR+BHwEHwQGBKkDXwOzAp0CugIHAhACwQF1ASgBEQHDANIAxwCdALkAigCcAHkAcwCNAIUAbQBWAG4AWgBkAEwAVgBIAEAAZwBNADsAUwBYADUARQBSACoAOgAuAFQANwBCADAAJAAjADgAGAA0ACQAMgAyADIAGAAZAC0AIgAmADAALgAWABgAGAAmACQADgAuACEAJAAcABoAFgAgABoAHgASABAAGgAcABgABgAmAAwACgAOABIAFgAYABYADAAMABAAEQAKABQADAAQAA4ACAAKAAwABgAAABIACgAGAAoACAAKAAwACgAKAAIAEAAGAAgABgAEAAYAAgAGAAoABAAEAAgAAAAEAAIAAAACAAIACgAAAAIAAAACAAQAAAAEAAIAAgAAAAQAAAAAAAQAAAAEAAAAAgACAAIAAgAAAAAAAgAGAAAAAgACAAAAAgAAAAAAAAAEAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACgAPABQAFgAYAAAAAAAAAAAAAAAAAAAABgAIAAYACAAIABAAFwAAAAAAAAAAAAAAAAAEAAwADgAMABEAGwAqACoAAAAAAAAAAAAAAAAABAAeAAwAIQAlAB4ALAAyAAAAAAAAAAAAAAAAAAQAEgAOABoAFgAYAB8AGgAAAAAAAAAAAAAAAAAIAAkADgASABgAFAAoAAoAAAAAAAAAAAAAAAAAAgAEAA0AEAALAAwAFgAYAAAAAAAAAAAAAAAAAAIACgAHAAoACAAEAAwADQAAAAAAAAAAAAAAAAAIAAUAAgAIAAoAAgAEABAAAAAAAAAAAAAAAAAABAAAAAIAAAAMAAgADgAEAAAAAAAAAAAAAAAAAAAAAAACAAIAAAAEAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAgAGAAAAAAAAAAAAAAAAAAAAAAACAAAAAAACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAAACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAAAMAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAgAIAAAAAAAAAAAAAAAAAA==||||||F\r";


            ChuLiOneHL7(str);

        }
    }
}
