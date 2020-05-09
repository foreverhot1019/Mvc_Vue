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
using AirOut.Web.Models;
using AirOut.Web.Services;
using AirOut.Web.Repositories;
using AirOut.Web.Extensions;
using System.IO;
using System.Reflection;


namespace AirOut.Web.Controllers
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
            //IWReceiptDetailService receiptService,
            //IWBookItemService bookitemService,
            //IPgaGrService pgagrService,
            //ICompanyService companyService,
            //IPgaKittingService kittingService,
            //IWOrderDetailService orderService,
            //ICodeItemService codeService,
            //IPgaSkuService pgaskuService,
            //IMergeDecConfigService mergeDecConfigService,
            //IHSCODEService HSCODEServiceService,
            //IWDecDetailService WDecDetailService,
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

                Type[] types = assem.GetTypes();
                foreach (Type type in types)
                {
                    if (type == null || type.Namespace == null || type.Name == null)
                        continue;

                    if (!(type.Namespace.ToUpper().IndexOf("AirOut") >= 0 || type.Name.ToUpper().IndexOf("AirOut") >= 0 || type.Name.IndexOf("<>c") < 0))
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

                #region 获取当前AirOut.Web.Models类的Type

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
                                        string ErrMsg = AirOut.Web.Extensions.Common.GetExceptionMsg(ex);
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

                //if (modelType == "WBookItem_GNo")
                //{
                //    _bookitemService.WriteG_NoDataTable(datatable);
                //    _unitOfWork.SaveChanges();
                //}

                //if (modelType == "WRecDtl_SCNo")
                //{
                //    ErrList = _receiptService.WriteSCNoDataTable(datatable);
                //    if (ErrList.Any())
                //    {
                //        return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);
                //    }
                //    //已经改成SQL执行方式，无需SaveChanges，时间过于缓慢
                //    //_unitOfWork.SaveChanges();
                //}

                //if (modelType == "WDecDetailEntryNo")
                //{
                //    ErrList = _WDecDetailService.WriteEntryNoDataTable(datatable);
                //    if (ErrList.Any())
                //    {
                //        return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);
                //    }
                //    //已经改成SQL执行方式，无需SaveChanges，时间过于缓慢
                //    //_unitOfWork.SaveChanges();
                //}

                //if (modelType == "WDecDetailG_No")
                //{
                //    ErrList = _WDecDetailService.WriteG_NoDataTable(datatable);
                //    if (ErrList.Any())
                //    {
                //        return Json(new { Success = false, ErrMsg = string.Join("，", ErrList) }, JsonRequestBehavior.AllowGet);
                //    }
                //    //已经改成SQL执行方式，无需SaveChanges，时间过于缓慢
                //    //_unitOfWork.SaveChanges();
                //}

                //if (modelType == "CodeItem")
                //{
                //    _codeService.SaveHead(datatable);
                //    _unitOfWork.SaveChanges();
                //    _codeService.ImportDataTable(datatable);
                //    _unitOfWork.SaveChanges();
                //}

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
    }
}
