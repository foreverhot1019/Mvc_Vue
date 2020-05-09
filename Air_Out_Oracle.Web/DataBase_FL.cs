using AirOut.Web.Extensions;
using AirOut.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace AirOut.Web
{
    public class DataBase_FL : System.Web.Hosting.IRegisteredObject
    {
        private string FolderPath;//程序 运行目录
        private string OPS_EttInforSQLPath;//委托数据
        private string BillArApSQLPath;//应收/付 头数据
        private string FileDownLoadPath;//程序 下载目录
        private string BillArApDtlSQLPath;//应收/付 明细数据 
        private string DataBaseFtp;//数据仓库Ftp地址（包括用户名和密码） 
        private Rar_FileHelper ORar_FileHelper;//压缩文件
        private FtpHelper OFtpHelper;//FTP帮助

        System.Threading.Timer OTime;
        private int period = 1000 * 60;//间隔事件 毫秒

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataBase_FL()
        {
            OPS_EttInforSQLPath = ConfigurationManager.AppSettings["OPS_EttInforSQLPath"] ?? "";
            BillArApSQLPath = ConfigurationManager.AppSettings["BillArApSQLPath"] ?? "";
            BillArApDtlSQLPath = ConfigurationManager.AppSettings["BillArApDtlSQLPath"] ?? "";
            FileDownLoadPath = ConfigurationManager.AppSettings["FileDownLoadPath"] ?? "\\FileDownLoadPath\\";
            DataBaseFtp = ConfigurationManager.AppSettings["DataBaseFtp"] ?? "";

            FolderPath = System.AppDomain.CurrentDomain.BaseDirectory;//.GetCurrentDirectory();//程序 运行目录
            if (!Directory.Exists(FolderPath + FileDownLoadPath))
                Directory.CreateDirectory(FolderPath + FileDownLoadPath);
            ORar_FileHelper = new Rar_FileHelper();
            OFtpHelper = new FtpHelper();
            if (!string.IsNullOrEmpty(DataBaseFtp))
            {
                var DataBaseFtpStr = DataBaseFtp.ToUpper();
                var idx = DataBaseFtpStr.LastIndexOf('@');
                if (idx > 0)
                {
                    var NamePsw = DataBaseFtp.Substring(0, idx);
                    OFtpHelper.Uri = new Uri("ftp://" + DataBaseFtp.Substring(idx + 1));
                    idx = NamePsw.LastIndexOf(':');
                    if (idx > 0)
                    {
                        OFtpHelper.UserName = NamePsw.Substring(0, idx).Substring(6);
                        OFtpHelper.Password = NamePsw.Substring(idx + 1);
                    }
                }
            }
            OTime = new System.Threading.Timer(new System.Threading.TimerCallback((x) =>
            {
                var day = DateTime.Now.ToString("yyyy/MM/dd");
                var date = DateTime.Now;
                var MinDate = DateTime.MinValue;
                var MaxDate = DateTime.MinValue;
                //string Msg = "OTime:" + date.ToString("yyyy-MM-dd HH:mm:ss");
                //Console.WriteLine("----------------"+Msg);
                if (!DateTime.TryParse(day + " 06:00:00", out MinDate)) MinDate = DateTime.MinValue;
                if (!DateTime.TryParse(day + " 07:00:00", out MaxDate)) MaxDate = DateTime.MinValue;
                var a = date <= MaxDate;
                var b = date >= MinDate;
                Common.WriteLog_Local("DataBase_FL-" + MinDate + "-" + date + "-" + MaxDate, "DataBase_FL\\Timer");
                if (MinDate != MaxDate && (a && b))
                {
                    PostOPS_EttInforToDataWareHouse();
                }
            }), null, 0, period);
        }

        /// <summary>
        /// 向数据仓库 抛 A_S10_OPS_ETTINFOR_20181126_20181127130000.csv.zip
        /// </summary>
        public void PostOPS_EttInforToDataWareHouse()
        {
            Dictionary<string, string> ArrDataStr = new Dictionary<string, string>() { 
                { "OPS_ENTRUSTMENTINFOR", OPS_EttInforSQLPath }, 
                { "Bms_Bill_ArAp", BillArApSQLPath }, 
                { "Bms_Bill_ArApDTL", BillArApDtlSQLPath } 
            };
            Dictionary<string, string> ArrSQL = new Dictionary<string, string>();
            try
            {
                string CSVFilePathFolder = FolderPath + FileDownLoadPath + "\\DataBase\\" + DateTime.Now.ToString("yyyy-MM-dd");
                var DirFolder = new DirectoryInfo(CSVFilePathFolder);
                if (DirFolder.Exists)
                {
                    var FileProfix = "A_S10_";
                    var ArrFile = DirFolder.GetFiles();
                    if(ArrFile.Any(x => x.Name.StartsWith(FileProfix))){
                        return;
                    }
                }

                foreach (var item in ArrDataStr)
                {
                    FileInfo OFile = new FileInfo(FolderPath + item.Value);
                    if (OFile.Exists)
                    {
                        string SQLStr = OFile.ReadToEnd();
                        if (!string.IsNullOrEmpty(SQLStr))
                        {
                            ArrSQL.Add(item.Key, SQLStr);
                        }
                        else
                        {
                            Common.WriteLogByLog4Net("抛" + item.Key + "-数据仓库出错，" + item.Key + "SQL语句，不存在", Common.Log4NetMsgType.Error);
                        }
                    }
                    else
                    {
                        Common.WriteLogByLog4Net("抛" + item.Key + "-数据仓库出错，" + item.Key + "SQL文件，不存在", Common.Log4NetMsgType.Error);
                    }
                }
                ArrSQL.Add("DelArAp_Dtl", "");
                var NowDate = DateTime.Now.AddDays(-1);//昨天
                foreach (var item in ArrSQL)
                {
                    var SQLStr = item.Value;
                    var ds = new System.Data.DataSet();
                    if (item.Key != "DelArAp_Dtl")
                        ds = SQLDALHelper.OracleHelper.GetDataSet(SQLStr);
                    else
                    {
                        var ORedisHelper = new RedisHelp.RedisHelper();
                        var RedisKeyDelArAp_Dtl = Common.RedisKeyDelArAp_Dtl + ":" + NowDate.ToString("yyyyMMdd");//删除的键值
                        System.Data.DataTable dtDel = new System.Data.DataTable();
                        dtDel.Columns.Add("DelStr");
                        var ArApdDelLen = ORedisHelper.ListLength(RedisKeyDelArAp_Dtl);
                        if (ArApdDelLen > 0)
                        {
                            var ArrArApdDelStr = ORedisHelper.ListRange<string>(RedisKeyDelArAp_Dtl, 0, -1);
                            foreach (var ArApdDelStr in ArrArApdDelStr)
                                dtDel.Rows.Add(ArApdDelStr);
                        }
                        ds.Tables.Add(dtDel);
                        //删除4天前的应收/付 删除的数据
                        ORedisHelper.KeyDelete(Common.RedisKeyDelArAp_Dtl + ":" + DateTime.Now.AddDays(-4).ToString("yyyyMMdd"));
                    }
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        string CSVFileName = "A_S10_" + item.Key.ToUpper() + "_" + NowDate.ToString("yyyyMMdd") + "_" + NowDate.ToString("yyyyMMddHHmmss") + ".0001.csv";//+ "_" +(new Random()).Next(1, 999).ToString("000") 
                        string CSVFilePath = FolderPath + FileDownLoadPath + "\\DataBase\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + CSVFileName;
                        CSVFileHelper.SaveCSV(dt, CSVFilePath);
                        var RarFile = ORar_FileHelper.AddFileToRar(new List<string> { CSVFilePath }, ".zip");
                        var NewFileName = CSVFileName + ".zip";
                        if (OFtpHelper.UploadFile(RarFile, NewFileName))
                        {
                            var flagFile = CSVFilePath + ".zip.flg";
                            FileInfo ORarFile = new FileInfo(RarFile);
                            var fs = File.Create(flagFile);
                            var StreamRW = new StreamWriter(fs);
                            StreamRW.Write(CSVFileName + ".zip|" + ORarFile.Length.ToString() + "|" + ds.Tables[0].Rows.Count + "|0");
                            StreamRW.Close();
                            if (!OFtpHelper.UploadFile(flagFile, NewFileName + ".flg"))
                            {
                                Common.WriteLogByLog4Net("抛" + item.Key + "-数据仓库出错，上传flag文件失败" + CSVFileName, Common.Log4NetMsgType.Error);
                            }
                        }
                        else
                            Common.WriteLogByLog4Net("抛" + item.Key + "-数据仓库出错，上传文件失败" + CSVFileName, Common.Log4NetMsgType.Error);
                    }
                    else
                    {
                        Common.WriteLogByLog4Net("抛" + item.Key + "-数据仓库出错，" + item.Key + "SQL语句，数据集为空", Common.Log4NetMsgType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogByLog4Net(ex, Common.Log4NetMsgType.Error);
            }
        }

        /// <summary>
        /// 委托数据
        /// </summary>
        public class OPS_EttInfor
        {
            public int Id { get; set; }
            public string Operation_Id { get; set; }
            public bool Is_TG { get; set; }
            public string Consignee_Code { get; set; }
            public string Consign_Code { get; set; }
            public string Entrustment_Name { get; set; }
            public string MBL { get; set; }
            public string HBL { get; set; }
            public string End_Port { get; set; }
            public string Airways_Code { get; set; }
            public string Book_Flat_Code { get; set; }
            public string Flight_No { get; set; }
            public DateTime? Flight_Date_Want { get; set; }
            public DateTime? ADDTS { get; set; }
            public string ADDWHO { get; set; }
            public string FWD_Code { get; set; }
            public decimal? Pieces_TS { get; set; }
            public decimal? Weight_TS { get; set; }
            public decimal? Volume_TS { get; set; }
            public decimal? Pieces_SK { get; set; }
            public decimal? Weight_SK { get; set; }
            public decimal? Volume_SK { get; set; }
            public decimal? Pieces_Fact { get; set; }
            public decimal? Weight_Fact { get; set; }
            public decimal? Volume_Fact { get; set; }
        }

        /// <summary>
        /// 应收应付 头数据
        /// </summary>
        public class BillArAp
        {
            public int Id { get; set; }
            public bool IsAr { get; set; }
            public bool IsMBLJS { get; set; }
            public string Dzbh { get; set; }
            public string Line_No { get; set; }
            public string Bill_Type { get; set; }
            public decimal Bill_Account2 { get; set; }
            public decimal Bill_Amount { get; set; }
            public decimal Bill_AmountTaxTotal { get; set; }
            public decimal Bill_TaxRate { get; set; }
            public bool Bill_HasTax { get; set; }
            public decimal Bill_TaxAmount { get; set; }
            public string Money_Code { get; set; }
            public string Bill_Object_Id { get; set; }
            public string Payway { get; set; }
            public string Remark { get; set; }
            public DateTime? Bill_Date { get; set; }
            public DateTime? Sumbmit_Date { get; set; }
            public DateTime? SignIn_Date { get; set; }
            public DateTime? Invoice_Date { get; set; }
            public DateTime? SellAccount_Date { get; set; }
            public bool Cancel_Status { get; set; }
        }

        /// <summary>
        /// 应收应付 明细数据
        /// </summary>
        public class BillArApDTL
        {
            public int Id { get; set; }
            public int Bms_Bill_Ar_Id { get; set; }
            public int Bms_Bill_Ap_Id { get; set; }
            public string Dzbh { get; set; }
            public string Line_No { get; set; }
            public int Line_Id { get; set; }
            public string Charge_Code { get; set; }
            public string Charge_Desc { get; set; }
            public decimal Unitprice2 { get; set; }
            public decimal Qty { get; set; }
            public decimal Account2 { get; set; }
            public string Money_Code { get; set; }
            public string Summary { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="immediate"></param>
        public void Stop(bool immediate)
        {
            OTime.Dispose();
        }
    }
}