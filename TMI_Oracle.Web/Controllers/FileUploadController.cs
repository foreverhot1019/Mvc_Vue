using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using Newtonsoft.Json;
using TMI.Web.Models;
using TMI.Web.Services;
using TMI.Web.Repositories;
using TMI.Web.Extensions;
using System.IO;
using System.Reflection;


namespace TMI.Web.Controllers
{
    public class FileUploadController : Controller
    {
        //private readonly ISKUService  _sKUService;
        //private readonly ICompanyService _companyService;
        //private readonly IPgaGrService _pgagrService;
        //private readonly IPgaKittingService _kittingService;
        //private readonly IPgaSkuService _pgaskuService;
        //private readonly IWBookItemService _bookitemService;
        //private readonly IWReceiptDetailService _receiptService;
        //private readonly IWOrderDetailService _orderService;
        //private readonly ICodeItemService _codeService;
        //private readonly IMergeDecConfigService _mergeDecConfigService;
        //private readonly IHSCODEService _HSCODEServiceService;
        //private readonly IWDecDetailService _WDecDetailService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDataTableImportMappingService _DataTableImportMappingService;

        public FileUploadController(
            IUnitOfWorkAsync unitOfWork,
            IDataTableImportMappingService DataTableImportMappingService)
        {
            //_iBOMComponentService = iBOMComponentService;
            //_sKUService  = sKUService;
            //_companyService = companyService;
            //_kittingService = kittingService;
            //_pgagrService = pgagrService;
            //_pgaskuService = pgaskuService;
            //_bookitemService = bookitemService;
            //_receiptService = receiptService;
            //_orderService = orderService;
            //_codeService = codeService;
            //_mergeDecConfigService = mergeDecConfigService;
            //_HSCODEServiceService = HSCODEServiceService;
            //_WDecDetailService = WDecDetailService;
            _unitOfWork = unitOfWork;
            _DataTableImportMappingService = DataTableImportMappingService;
        }

        //回单文件上传 文件名格式 回单+_+日期+_原始文件
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase Filedata, int ID = 0)//增加参数ID --By liyu
        {
            WebdbContext dbcontext = new WebdbContext();
            //类名称
            string modelType = "";
            //要执行的类服务中的 方法名
            string ServiceFucName = "ImportDataTable";
            //是否是SQL语句方法
            bool IsSQLFuc = false;
            //错误信息
            List<string> ErrList = new List<string>();
            try
            {
                // 如果没有上传文件
                if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                {
                    return this.HttpNotFound();
                }
                modelType = this.Request.Form["modelType"];
                if (this.Request.Form["ServiceFucName"] != null)
                {
                    if (!string.IsNullOrEmpty(this.Request.Form["ServiceFucName"].ToString()))
                    {
                        ServiceFucName = this.Request.Form["ServiceFucName"].ToString();
                    }
                }
                if (this.Request.Form["IsSQLFuc"] != null)
                {
                    if (!string.IsNullOrEmpty(this.Request.Form["IsSQLFuc"].ToString()))
                    {
                        IsSQLFuc = true;
                    }
                }
                DataSet dsImport = ExcelHelper.GetDataTableFromExcelByAspose(Filedata.InputStream);
                DataTable datatable = null;// ExcelHelper.GetDataTableFromExcel(Filedata.InputStream);
                if (dsImport != null)
                {
                    if (dsImport.Tables.Count > 0)
                    {
                        datatable = dsImport.Tables[0];
                    }
                    else
                    {
                        ErrList.Add("文件数据提取错误");
                        return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);//2017-3-13
                    }
                }
                else
                {
                    ErrList.Add("文件数据提取错误");
                    return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);//2017-3-13 
                }

                Dictionary<string, List<string>> dictNameSpace_Class = new Dictionary<string, List<string>>();
                string TopNameSpace = "AirOut.Web";
                string ServiceNameSpace = "Services";
                string ModelsNameSpace = "Models";
                //当前类的Type
                Type TClass = null;
                //Repository.Pattern.Ef6.Repository<T> 的实例
                object objRep = null;
                //当前执行代码的程序集
                Assembly assem = Assembly.GetExecutingAssembly();
                ////获取需要执行的类的Type
                //TClass = assem.GetType(TopNameSpace + "." + ModelsNameSpace + "." + modelType);

                #region 获取 网站 所有命名空间AirOut开头的类

                List<Type> types = (List<Type>)CacheHelper.Get_SetCache(Common.CacheNameS.AllEntityAssembly);
                foreach (Type type in types)
                {
                    if (type == null || type.Namespace == null || type.Name == null)
                        continue;

                    if (!(type.Namespace.ToUpper().IndexOf("AIROUT") >= 0 || type.Name.ToUpper().IndexOf("AIROUT") >= 0 || type.Name.IndexOf("<>c") < 0))
                        continue;

                    var WheredictNameSpace_Class = dictNameSpace_Class.Where(x => x.Key == type.Namespace);
                    if (WheredictNameSpace_Class.Any())
                    {
                        var ClassValue = WheredictNameSpace_Class.FirstOrDefault().Value;
                        if (!ClassValue.Any(x => x == type.Name))
                            ClassValue.Add(type.Name);
                    }
                    else
                    {
                        dictNameSpace_Class.Add(type.Namespace, new List<string> { type.Name });
                    }
                }

                #endregion

                #region 获取当前TMI.Web.Models类的Type

                var Where_dictNameSpace_Class = dictNameSpace_Class.Where(x => x.Key == (TopNameSpace + "." + ModelsNameSpace));
                if (Where_dictNameSpace_Class.Any())
                {
                    var ArrClass = Where_dictNameSpace_Class.FirstOrDefault().Value.Where(x => x.ToUpper() == modelType.ToUpper());
                    if (ArrClass.Any())
                    {
                        TClass = assem.GetType(TopNameSpace + "." + ModelsNameSpace + "." + ArrClass.FirstOrDefault());
                    }
                }

                #endregion

                #region 获取Repository.Pattern.Ef6.Repository<T> 的实例

                if (TClass != null)
                {
                    Type Type_Rep = typeof(Repository.Pattern.Ef6.Repository<>);
                    Type TypeRep = Type_Rep.MakeGenericType(TClass);
                    List<object> arrRepParam = new List<object>();
                    arrRepParam.Add(dbcontext);
                    arrRepParam.Add(_unitOfWork);
                    objRep = Activator.CreateInstance(TypeRep, arrRepParam.ToArray());
                }

                #endregion

                Where_dictNameSpace_Class = dictNameSpace_Class.Where(x => x.Key == (TopNameSpace + "." + ServiceNameSpace));
                if (Where_dictNameSpace_Class.Any())
                {
                    var ArrServiceClass = Where_dictNameSpace_Class.FirstOrDefault().Value.Where(x => x.ToUpper().Contains(modelType.ToUpper()));
                    if (ArrServiceClass.Any())
                    {
                        ArrServiceClass = ArrServiceClass.Where(x => !x.StartsWith("I"));
                        if (ArrServiceClass.Any())
                        {
                            //获取服务类
                            Type type = assem.GetType((TopNameSpace + "." + ServiceNameSpace) + "." + ArrServiceClass.FirstOrDefault());
                            if (type != null)
                            {
                                List<object> arr = new List<object>();
                                arr.Add(objRep);
                                arr.Add(_DataTableImportMappingService);

                                ////获取构造函数
                                //ConstructorInfo[] ConInfoS = type.GetConstructors();
                                ////获取构造函数参数
                                //ParameterInfo[] param_s = ConInfoS[0].GetParameters();

                                object objtemp = Activator.CreateInstance(type, arr.ToArray());
                                //MethodInfo[] Methods = type.GetMethods();
                                MethodInfo Method = type.GetMethod(ServiceFucName);
                                if (Method != null)
                                {
                                    List<object> ArrParamterInfo = new List<object>();
                                    //ParameterInfo[] ParameterInfos = Method.GetParameters();
                                    //foreach (ParameterInfo pfi in ParameterInfos)
                                    //{
                                    //    ArrParamterInfo.Add(pfi.GetType().ToString());
                                    //}
                                    ArrParamterInfo = new List<object>();
                                    ArrParamterInfo.Add(datatable);
                                    //增加参数ID --By liyu
                                    if (!ID.Equals(0))
                                    {
                                        ArrParamterInfo.Add(ID);
                                    }
                                    //如果是方法参数是泛型
                                    Type retType = Method.ReturnParameter.ParameterType;
                                    if (retType.IsGenericType)
                                    {

                                    }
                                    try
                                    {
                                        var retobj = Method.Invoke(objtemp, ArrParamterInfo.ToArray());
                                        if (!IsSQLFuc)
                                        {
                                            #region 获取导入方法的返回值

                                            if (Method.ReturnType == typeof(System.String))
                                            {
                                                if (!string.IsNullOrEmpty(retobj.ToString()))
                                                {
                                                    return Json(new { Success = false, ErrMsg = retobj.ToString() }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            if (Method.ReturnType == typeof(List<System.String>))
                                            {
                                                if (((List<System.String>)retobj).Any())
                                                {
                                                    return Json(new { Success = false, ErrMsg = string.Join("<br/>", ((List<System.String>)retobj)) }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            if (Method.ReturnType == typeof(System.Boolean))
                                            {
                                                if (!(bool)retobj)
                                                {
                                                    return Json(new { Success = false, ErrMsg = "失败" }, JsonRequestBehavior.AllowGet);
                                                }
                                            }

                                            #endregion

                                            //反射变更保存到数据库
                                            dbcontext.SaveChanges();
                                            //unitofwork变更保存到数据库
                                            _unitOfWork.SaveChanges();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string ErrMsg = TMI.Web.Extensions.Common.GetExceptionMsg(ex);
                                        ErrList.Add(ErrMsg);
                                    }
                                }
                                else
                                {
                                    ErrList.Add("要执行的方法不存在");
                                }
                            }
                            else
                            {
                                ErrList.Add("服务不存在");
                            }
                        }
                        else
                        {
                            ErrList.Add("服务不存在");
                        }
                    }
                }
                if (ErrList.Any())
                {
                    return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);
                }

                #region 保存文件

                string uploadfilename = System.IO.Path.GetFileName(Filedata.FileName);
                string folder = System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"] == null ? "/FileUpLoad/" : System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"].ToString();
                folder = folder + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                folder = Server.MapPath(folder);
                string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");//获取时间
                string newFileName = string.Format("{0}_{1}", time, uploadfilename);//重组成新的文件名

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                else
                {
                    //string LastFile = Server.MapPath(Lastfilename);
                    //FileInfo nmmFile = new FileInfo(LastFile);
                    //if (nmmFile.Exists)
                    //{
                    //    nmmFile.Delete();
                    //}
                }

                string virtualPath = string.Format("{0}\\{1}", folder, newFileName);

                Filedata.SaveAs(virtualPath);

                #endregion

                return Json(new { Success = true, filename = folder + newFileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                string ErrMsg = Common.GetExceptionMsg(e);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileContentResult DownLoadFile(string FilePath = "")
        {
            byte[] fileContent = null;
            string FileName = "";
            string mimeType = "";
            if (!string.IsNullOrEmpty(FilePath))
            {
                FileInfo nmmFile = new FileInfo(Server.MapPath(FilePath));
                if (nmmFile.Exists)
                {
                    FileName = nmmFile.Name.Substring(nmmFile.Name.LastIndexOf('_') + 1);
                    mimeType = GetMimeType(nmmFile.Extension);
                    fileContent = new byte[Convert.ToInt32(nmmFile.Length)];
                    FileStream fs = nmmFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                    fs.Read(fileContent, 0, Convert.ToInt32(nmmFile.Length));
                    fs.Dispose();
                    fs.Close();
                }
            }
            if (fileContent.Length > 0 && !string.IsNullOrEmpty(mimeType) && !string.IsNullOrEmpty(FileName))
                return File(fileContent, mimeType, FileName);
            else
                return null;
        }

        private string GetMimeType(string fileExtensionStr)
        {
            string ContentTypeStr = "application/octet-stream";
            string fileExtension = fileExtensionStr.ToLower();
            switch (fileExtension)
            {
                case ".mp3":
                    ContentTypeStr = "audio/mpeg3";
                    break;
                case ".mpeg":
                    ContentTypeStr = "video/mpeg";
                    break;
                case ".jpg":
                    ContentTypeStr = "image/jpeg";
                    break;
                case ".bmp":
                    ContentTypeStr = "image/bmp";
                    break;
                case ".gif":
                    ContentTypeStr = "image/gif";
                    break;
                case ".doc":
                    ContentTypeStr = "application/msword";
                    break;
                case ".css":
                    ContentTypeStr = "text/css";
                    break;
                case ".html":
                    ContentTypeStr = "text/html";
                    break;
                case ".htm":
                    ContentTypeStr = "text/html";
                    break;
                case ".swf":
                    ContentTypeStr = "application/x-shockwave-flash";
                    break;
                case ".exe":
                    ContentTypeStr = "application/octet-stream";
                    break;
                case ".inf":
                    ContentTypeStr = "application/x-texinfo";
                    break;
                default:
                    ContentTypeStr = "application/octet-stream";
                    break;
            }
            return ContentTypeStr;
        }

        #region 附件处理

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="postfile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAttach()
        {
            try
            {
                HttpPostedFileBase Filedata = Request.Files[0];
                //是否存多个
                int IsMany = Request["ismany"] == null ? 0 : Convert.ToInt32(Request["ismany"]);
                //附件数据Guid,多个附件关联时，此Id必须传
                string FileGuid = Request["FileGuid"] == null ? Guid.NewGuid().ToString() : Request["FileGuid"].ToString();

                #region 文件信息

                //文件大小 单位byte
                long dataLen = Filedata.InputStream.Length;
                //文件二进制流
                byte[] bytedata = new byte[dataLen];
                Filedata.InputStream.Read(bytedata, 0, (int)dataLen);
                //文件真实类型
                string FileClass = Common.GetFileClass(bytedata);
                //文件真实类型是否是图片
                bool FileIsIamge = Common.IsImage(FileClass);
                //图片缩略图 二进制流
                byte[] PicMinByteData = null;
                if (FileIsIamge)
                {
                    System.IO.MemoryStream fs = new System.IO.MemoryStream((byte[])bytedata);
                    System.IO.MemoryStream newst = (System.IO.MemoryStream)Common.CutImageForCustom(fs, 64, 64, 50);
                    PicMinByteData = newst.ToArray();
                    newst.Close();
                    fs.Close();
                }
                //文件名称
                string FileName = Filedata.FileName;
                //文件扩展名
                string FileExtension = System.IO.Path.GetExtension(FileName);
                var EnumDic = Common.GetEnumToDic("ImageFileClass");
                if (EnumDic.Any())
                {
                    var WhereEnumDic = EnumDic.Where(x => x.Value.ToString() == FileClass);
                    if (WhereEnumDic.Any())
                    {
                        FileExtension = "." + WhereEnumDic.FirstOrDefault().Key.ToString();
                    }
                }
                //文件存放文件夹路径
                string FileDir = (CacheHelper.Get_SetStringConfAppSettings("/FileUpload/AddAttach", "FileUpLoadPath") ?? "/FileUpLoad/").ToString();// System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"] == null ? "/FileUpLoad/" : System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"].ToString();
                FileDir += DateTime.Now.ToString("yyyy-MM");
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(HttpContext.Server.MapPath(FileDir));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                var NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff_" + (new Random()).Next(1, 99).ToString("00")) + FileExtension;
                //文件存放路径
                string FilePath = dirInfo.FullName + "/" + NewFileName;
                System.IO.FileInfo OFileInfo = new System.IO.FileInfo(FilePath);
                Filedata.SaveAs(FilePath);

                #endregion

                #region 创建附件

                var FileAttachRep = _unitOfWork.Repository<FileATTACH>();

                bool IsAdd = true;

                FileATTACH OFileATTACH = new FileATTACH();
                if (IsMany <= 0)
                {
                    var WhereFileAttach = FileAttachRep.Query(x => x.FileGuid == FileGuid).Select();
                    if (WhereFileAttach.Any())
                    {
                        OFileATTACH = WhereFileAttach.FirstOrDefault();
                        OFileATTACH.PicMinData = null;
                        IsAdd = false;
                    }
                }
                OFileATTACH.OriginFileName = FileName;
                OFileATTACH.FileExtension = FileExtension.ToLower();
                OFileATTACH.FileGuid = FileGuid;
                OFileATTACH.FileLength = Convert.ToDecimal(dataLen);
                OFileATTACH.FileData = bytedata;
                OFileATTACH.ADDTS = DateTime.Now;
                OFileATTACH.ADDWHO = Utility.CurrentAppUser.UserName;
                OFileATTACH.FileName = NewFileName;
                OFileATTACH.FilePath = FilePath;
                if (PicMinByteData != null)
                    OFileATTACH.PicMinData = PicMinByteData;
                if (IsAdd)
                    FileAttachRep.Insert(OFileATTACH);
                else
                    FileAttachRep.Update(OFileATTACH);

                #endregion

                _unitOfWork.SaveChanges();

                return Json(new { Success = true, IsImage = GetByteIsImageByByte(bytedata), FileGuid = FileGuid, FileAttachID = OFileATTACH.ID }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                var ErrMsg = Common.GetExceptionMsg(e);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="FileAttachID"></param>
        /// <returns></returns>
        public ActionResult DeleteAttach(string FileGuid="",int FileAttachID = 0)
        {
            if (!string.IsNullOrEmpty(FileGuid))
            {
                var FileATTACHRep = _unitOfWork.Repository<FileATTACH>();
                var QFileATTACH = FileATTACHRep.Queryable().Where(x => x.FileGuid == FileGuid || x.ID == FileAttachID).ToList();
                if (QFileATTACH.Any())
                {
                    foreach (var item in QFileATTACH)
                    {
                        FileATTACHRep.Delete(item.ID);
                    }
                    _unitOfWork.SaveChanges();
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "数据已被删除" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Success = false, ErrMsg = "数据ID不存在" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取 二进制数据
        /// 图片 直接显示
        /// 文件 放<a>标签 下载
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetByteDataBybyte_Column(byte[] byteobj, string FileGuid = "", string FileAttachID = "")
        {
            string retHTML = "";
            bool IsImg = GetByteIsImageByByte(byteobj);
            string targetUrl = "/FileUpload/GetFileByte?FileGuid=" + FileGuid + "&FileAttachID=" + FileAttachID;
            if (IsImg)
            {
                string picMinUrl = "/FileUpload/GetImgMinByte?FileGuid=" + FileGuid + "&FileAttachID=" + FileAttachID;
                retHTML = "<div class=\"File_Image_div\" style=\"position: relative; width:64px; height:64px; margin: auto;\" title='双击下载原图'> \r\n" +
                           "     <img class=\"file_img\" src=\"" + picMinUrl + "\" targetUrl=\"" + targetUrl + "\" FileGuid='" + FileGuid + "' style=\"position: relative; width:64px; height:64px;\" ondblclick=\"var targetUrl = $(this).attr('targetUrl'); window.open(targetUrl, 'newWindow');\" /> \r\n" +
                           "     <div class=\"divX\" style=\"width:16px; height:16px;\" deltID='" + FileAttachID + "' onclick='deleteFile(this,\"" + FileGuid + "\");'> \r\n" +
                           "         <img src=\"/Images/delete.png\" style=\"width:16px; height:16px;\" /> \r\n" +
                           "     </div>" +
                           "</div>";
                //"<img style=' width:30px; height:30px' src='/EMS_HEADS/GetImgByte?id=" + id + "&column=" + column + "&columnVal=" + columnVal + "&FileAttachID=" + FileAttachID + "'>";
            }
            else
            {
                string FileUrl = "/Images/file.png";
                retHTML = "<div class=\"File_Image_div\" style=\"position: relative; width:64px; height:64px; margin: auto;\" title='双击下载文件'> \r\n" +
                           "     <img class=\"file_img\" src=\"" + FileUrl + "\" targetUrl=\"" + targetUrl + "\" FileGuid='" + FileGuid + "' style=\"position: relative; width:64px; height:64px;\" ondblclick=\"var targetUrl = $(this).attr('targetUrl'); window.open(targetUrl, 'newWindow');\" /> \r\n" +
                           "     <div class=\"divX\" style=\"width:16px; height:16px;\" deltID='" + FileAttachID + "' onclick='deleteFile(this,\"" + FileGuid + "\");'> \r\n" +
                           "         <img src=\"/Images/delete.png\" style=\"width:16px; height:16px;\" /> \r\n" +
                           "     </div>" +
                           "</div>";
                //"<a target='_blank' href='/EMS_HEADS/GetFileByte?id=" + id + "&column=" + column + "&columnVal=" + columnVal + "&FileAttachID=" + FileAttachID + "'>下载文件</a>";
            }
            return retHTML;
        }

        ///// <summary>
        ///// 获取所有二进制字段 显示HTML
        ///// </summary>
        ///// <param name="ids">所有列ID</param>
        ///// <param name="bytecolumns">二进制数据列</param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult GetbyteColumnHtml(string[] ids, string[] bytecolumns)
        //{
        //    List<object> arr = new List<object>();
        //    if (ids == null)
        //        return Json(arr, JsonRequestBehavior.AllowGet);
        //    else if (ids.Count() <= 0)
        //        return Json(arr, JsonRequestBehavior.AllowGet);
        //    if (bytecolumns == null)
        //        return Json(arr, JsonRequestBehavior.AllowGet);
        //    else if (bytecolumns.Count() <= 0)
        //        return Json(arr, JsonRequestBehavior.AllowGet);

        //    string SQLStr = "select ID," + string.Join(",", bytecolumns).ToUpper() + " from EMS_HEADS where id in('" + string.Join("','", ids) + "')";
        //    string SQLStr1 = "select t.* from FileAttachS t where ";
        //    for (int i = 0; i < bytecolumns.Count(); i++)
        //    {
        //        if (i == 0)
        //            SQLStr1 += " t.FileGuid in (select " + bytecolumns[i].ToUpper() + " from EMS_HEADS where id in('" + string.Join("','", ids) + "'))";
        //        else
        //            SQLStr1 += "or t.FileGuid in (select " + bytecolumns[i].ToUpper() + " from EMS_HEADS where id in('" + string.Join("','", ids) + "'))";
        //    }
        //    DataSet dsEMS_HEAD = SQLDALHelper.OracleHelper.GetDataSet(SQLStr);
        //    if (dsEMS_HEAD != null)
        //    {
        //        if (dsEMS_HEAD.Tables.Count > 0)
        //        {
        //            DataTable dtEMS_HEAD = dsEMS_HEAD.Tables[0];

        //            DataSet dsFileAttach = SQLDALHelper.OracleHelper.GetDataSet(SQLStr1);
        //            DataTable dtFileAttach = dsFileAttach.Tables[0];
        //            DataRow[] drs;

        //            foreach (DataRow dr in dtEMS_HEAD.Rows)
        //            {
        //                dynamic dobj = new System.Dynamic.ExpandoObject();
        //                int ID = Convert.ToInt32(dr["ID"].ToString());
        //                var dic = (IDictionary<string, object>)dobj;
        //                dic["ID"] = ID;
        //                foreach (var item in dtEMS_HEAD.Columns)
        //                {
        //                    if (item.ToString().ToUpper() != "ID")
        //                    {
        //                        drs = dtFileAttach.Select(" FileGuid = '" + dr[item.ToString()] + "'");
        //                        List<string> ArrHtmlStr = new List<string>();
        //                        if (drs.Any())
        //                        {
        //                            foreach (DataRow dr1 in drs)
        //                            {
        //                                ArrHtmlStr.Add(GetByteDataBybyte_Column((byte[])dr1["filedata"], ID, item.ToString(), dr[item.ToString()].ToString(), dr1["ID"].ToString()));
        //                            }
        //                        }
        //                        dic[item.ToString()] = "";
        //                        if (ArrHtmlStr.Any())
        //                        {
        //                            string HtmlStr = "";
        //                            HtmlStr += string.Join("\r\n", ArrHtmlStr);
        //                            dic[item.ToString()] = HtmlStr;
        //                        }
        //                    }
        //                }
        //                arr.Add(dobj);
        //            }
        //        }
        //    }

        //    return Json(arr, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 判断数据是否图片
        /// </summary>
        /// <param name="bytedata"></param>
        /// <returns></returns>
        public bool GetByteIsImageByByte(byte[] bytedata)
        {
            if (bytedata.Length > 0)
            {
                //string FileType = Common.GetByteType(id,(byte[])bytedata);
                return Common.GetByteIsImage(0, bytedata);
            }
            else
                return false;
        }

        /// <summary>
        /// 显示大图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="column"></param>
        public void GetImgByte(string FileGuid = "", int FileAttachID = 0)
        {
            try
            {
                var FileATTACHRep = _unitOfWork.Repository<FileATTACH>();
                if (!string.IsNullOrEmpty(FileGuid))
                {
                    IEnumerable<FileATTACH> q_FileAttach;
                    q_FileAttach = FileATTACHRep.Query(x => x.FileGuid == FileGuid).Select();
                    if (q_FileAttach.Any())
                    {
                        if (!q_FileAttach.Any(x => x.ID == FileAttachID))
                        {
                            q_FileAttach = q_FileAttach.Take(1);
                        }
                        else
                            q_FileAttach = q_FileAttach.Where(x => x.ID == FileAttachID);

                        if (q_FileAttach.Any())
                        {
                            var OFileAttach = q_FileAttach.FirstOrDefault();
                            if (OFileAttach.FileData == null || OFileAttach.FileData.Length <= 0)
                                Response.BinaryWrite(new byte[0]);
                            else
                            {
                                Response.BinaryWrite(OFileAttach.FileData);
                            }
                        }
                        else
                            Response.BinaryWrite(new byte[0]);
                    }
                    else
                        Response.BinaryWrite(new byte[0]);
                }
                else
                    Response.BinaryWrite(new byte[0]);
            }
            catch (Exception)
            {
                Response.BinaryWrite(new byte[0]);
            }
        }

        /// <summary>
        /// 显示小图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="column"></param>
        public void GetImgMinByte(string FileGuid = "", int FileAttachID = 0)
        {
            try
            {
                var FileATTACHRep = _unitOfWork.Repository<FileATTACH>();
                if (!string.IsNullOrEmpty(FileGuid))
                {
                    IEnumerable<FileATTACH> q_FileAttach;
                    q_FileAttach = FileATTACHRep.Query(x => x.FileGuid == FileGuid).Select();
                    if (q_FileAttach.Any())
                    {
                        if (!q_FileAttach.Any(x => x.ID == FileAttachID))
                        {
                            q_FileAttach = q_FileAttach.Take(1);
                        }
                        else
                            q_FileAttach = q_FileAttach.Where(x => x.ID == FileAttachID);

                        if (q_FileAttach.Any())
                        {
                            var OFileAttach = q_FileAttach.FirstOrDefault();
                            if (OFileAttach.PicMinData == null || OFileAttach.PicMinData.Length <= 0)
                                Response.BinaryWrite(OFileAttach.FileData);
                            else
                                Response.BinaryWrite(OFileAttach.PicMinData);
                        }
                        else
                            Response.BinaryWrite(new byte[0]);
                    }
                    else
                        Response.BinaryWrite(new byte[0]);
                }
                else
                    Response.BinaryWrite(new byte[0]);
            }
            catch (Exception)
            {
                Response.BinaryWrite(new byte[0]);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public ActionResult GetFileByte(string FileGuid = "", int FileAttachID = 0)
        {
            var FileATTACHRep = _unitOfWork.Repository<FileATTACH>();
            if (!string.IsNullOrEmpty(FileGuid))
            {
                IEnumerable<FileATTACH> q_FileAttach;
                q_FileAttach = FileATTACHRep.Query(x => x.FileGuid == FileGuid).Select();
                 
                if (!q_FileAttach.Any(x => x.ID == FileAttachID))
                {
                    q_FileAttach = q_FileAttach.Take(1);
                }
                else
                    q_FileAttach = q_FileAttach.Where(x => x.ID == FileAttachID);
                
                if (q_FileAttach.Any())
                {
                    byte[] bytedata = q_FileAttach.FirstOrDefault().FileData;
                    return File(bytedata, "application/octet-stream", q_FileAttach.FirstOrDefault().FileName);
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 取 html中img a标签中的 src或href url中的 参数
        /// </summary>
        /// <param name="HtmlStr">要获取 img a标签的 html字符串</param>
        /// <returns>img a标签中的 src或href 以及 url中的 参数</returns>
        public ActionResult GetHtmlParam(string htmlstr)
        {
            var ret = Common.Get_A_Img_Src(htmlstr);
            return Json(new { Success = true, result = ret }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
