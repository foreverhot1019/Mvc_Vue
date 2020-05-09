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
using AirOut.Web.Models;
using AirOut.Web.Services;
using AirOut.Web.Repositories;
using AirOut.Web.Extensions;
using System.Xml;
using System.ComponentModel.DataAnnotations;

namespace AirOut.Web.Controllers
{
    public class SetDefaultsesController : Controller
    {
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly string XMLPath;

        public SetDefaultsesController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
            XMLPath = System.Configuration.ConfigurationManager.AppSettings["SetDefaultsXml"] ?? "/App_Data/SetDefaults.xml";
        }

        // GET: SetDefaultses/SetDefaultsIndex
        public ActionResult Index()
        {
            if (HttpContext.Cache[Common.GeCacheEnumByName("SetDefaults").ToString()] == null)
            {
                Set_UpdateXMLCache();
            }

            return View();
        }

        // Get :SetDefaultses/SetDefaultsPageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult PageList(int offset = 0, int limit = 10, string search = "", string sort = "", string order = "", string TableNameChs = "", string ColumnNameChs = "")
        {
            var page = Request["page"] ?? "";
            if (!string.IsNullOrEmpty(page))
            {
                int ipage = 0;
                if (!int.TryParse(page, out ipage))
                {
                    var pagelist1 = new { total = 0 };
                    return Json(pagelist1, JsonRequestBehavior.AllowGet);
                }
                offset = (ipage - 1) * 10;
            }
            limit = int.Parse(Request.Params["rows"]);
            int totalCount = 0;
            int pagenum = offset / limit + 1;


            List<SetDefaults> ArrSetDefaults = new List<SetDefaults>();
            if (HttpContext.Cache[Common.GeCacheEnumByName("SetDefaults").ToString()] != null)
            {
                ArrSetDefaults = HttpContext.Cache.Get(Common.GeCacheEnumByName("SetDefaults").ToString()) as List<SetDefaults>;
            }

            if (ArrSetDefaults.Any())
            {
                if (search != "")
                {
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.ColumnNameChs.Contains(search) || x.TableNameChs.Contains(search) || x.DefaultValue.Contains(search)).ToList();
                }
                if (TableNameChs != "")
                {
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.TableNameChs.Contains(TableNameChs)).ToList();
                }
                if (ColumnNameChs != "")
                {
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.ColumnNameChs.Contains(ColumnNameChs)).ToList();
                }
                if (Request["ColumnName"] != "" && Request["ColumnName"] != null)
                {
                    var def = Request.Params["ColumnName"].ToString();
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.ColumnName.Contains(def)).ToList();
                }
                if (Request["TableName"] != "" && Request["TableName"] != null)
                {
                    var def = Request.Params["TableName"].ToString();
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.TableName.Contains(def)).ToList();
                }
                if (Request["DefaultValue"] != "" && Request["DefaultValue"] != null)
                {
                    var def = Request.Params["DefaultValue"].ToString();
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.DefaultValue.Contains(def)).ToList();
                }
                if (Request.Params["DataType"] != "" && Request.Params["DataType"] != null)
                {
                    ArrSetDefaults = ArrSetDefaults.Where(x => x.DataType.ToLower().Contains(Request.Params["DataType"].ToString().ToLower())).ToList();
                }
            }
            totalCount = ArrSetDefaults.Count();
            var rows = ArrSetDefaults.Skip(offset).Take(limit);

            var pagelist = new { total = totalCount, rows = rows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

        // GET: SetDefaultses/SetDefaultsDetails/5
        public ActionResult Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //SetDefaults setDefaults = _setDefaultsService.Find(id);
            //if (setDefaults == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        // GET: SetDefaultses/SetDefaultsCreate
        public ActionResult Create()
        {
            ViewBag.DefaultTables = getXMLDefaultTables();
            List<SelectListItem> ListTableColumns = new List<SelectListItem>();
            SelectListItem SltItem = new SelectListItem();
            SltItem.Value = "";
            SltItem.Text = "--请选择--";
            ListTableColumns.Add(SltItem);
            ViewBag.TableColumns = ListTableColumns;
            return View(new SetDefaults());
        }

        public ActionResult GetTableColumnByTableName(string TableName = "")
        {
            List<SelectListItem> ListColumns = GetAllColumnByTableName(TableName);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true, rows = ListColumns }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


        public ActionResult GetTableColumnByTableNameData(string TableName = "")
        {
            List<SelectListItem> ListColumns = GetAllColumnByTableName(TableName);
            if (Request.IsAjaxRequest())
            {
                var rows = ListColumns.Select(n => new { Value = n.Value, Text = n.Text });
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult GetTableColumnByTableNameDataQ(string q = "")
        {
            List<SelectListItem> ListColumns = GetAllColumnByTableName(q,"1");
            if (Request.IsAjaxRequest())
            {
                var rows = ListColumns.Select(n => new { Value = n.Value, Text = n.Text });
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


        public ActionResult GetDataType_Table_Column(string TableName = "", string ColumnName = "")
        {
            string DataTypeStr = GetDataTypeByTable_Column(TableName, ColumnName);
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true, DataTypeStr = DataTypeStr }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        // POST: SetDefaultses/SetDefaultsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(SetDefaults setDefaults)
        {
            if (ModelState.IsValid)
            {
                List<string> ErrorList = AddXMLBySetDefaults(setDefaults);
                if (ErrorList.Any())
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = string.Join("", ErrorList) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                DisplaySuccessMessage("Has append a SetDefaults record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(setDefaults);
        }

        // GET: SetDefaultses/SetDefaultsEdit/5
        public ActionResult Edit(string TableName = "", string ColumnName = "")
        {
            ViewBag.DefaultTables = getXMLDefaultTables();
            ViewBag.TableColumns = GetAllColumnByTableName(TableName);
            if (TableName == "" || ColumnName == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SetDefaults setDefaults = getSetDefaultsByTable_Column(TableName, ColumnName);
            if (setDefaults == null)
            {
                return HttpNotFound();
            }
            return View(setDefaults);
        }

        // POST: SetDefaultsesSetDefaults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(SetDefaults setDefaults)
        {
            if (ModelState.IsValid)
            {
                List<string> ErrorList = UpdateXMLBySetDefaults(setDefaults);
                if (ErrorList.Any())
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = string.Join("", ErrorList) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = true, ErrMsg = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                DisplaySuccessMessage("Has update a SetDefaults record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(setDefaults);
        }
        // GET: SetDefaultses/SetDefaultsDelete/5
        public ActionResult Delete(string TableName = "", string ColumnName = "")
        {
            if (TableName == "" || ColumnName == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //SetDefaults setDefaults = _setDefaultsService.Find(id);
            //if (setDefaults == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        // POST: SetDefaultses/SetDefaultsDelete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(SetDefaults[] ArrSetDefaults)
        {
            Dictionary<string, List<String>> dictError = new Dictionary<string, List<string>>();
            foreach (var item in ArrSetDefaults)
            {
                string TableName = item.TableName;
                string ColumnName = item.ColumnName;
                if (TableName == "" || ColumnName == "")
                {
                    List<string> ErrorList = new List<string>();
                    ErrorList.Add("参数不正确");
                    dictError.Add((TableName + "_" + ColumnName), ErrorList);
                }
                else
                {
                    try
                    {
                        List<string> ErrorList = DeleteByTable_Column(TableName, ColumnName);
                        if (ErrorList.Any())
                        {
                            dictError.Add((TableName + "_" + ColumnName), ErrorList);
                        }
                        else
                            Set_UpdateXMLCache();
                    }
                    catch (Exception e)
                    {
                        List<string> ErrorList = new List<string>();
                        ErrorList.Add(e.Message);
                        dictError.Add((TableName + "_" + ColumnName), ErrorList);
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                if (dictError.Any())
                {
                    return Json(new { Success = false, ErrMsg = dictError }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = true, ErrMsg = dictError }, JsonRequestBehavior.AllowGet);
                }
            }
            DisplaySuccessMessage("Has delete a SetDefaults record");
            return RedirectToAction("Index");
        }

        #region XML 相关操作

        //设置或者更新XML数据缓存
        public void Set_UpdateXMLCache()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Server.MapPath(XMLPath));
            XmlNodeList NodeList = xml.SelectNodes("WebSettings/table/AutoSet");

            List<SetDefaults> ArrSetDefaults = new List<SetDefaults>();

            foreach (XmlNode item in NodeList)
            {
                SetDefaults OSetDefaults = new SetDefaults();
                OSetDefaults.TableName = item.ParentNode == null ? "" : item.ParentNode.Attributes["name"].Value;
                OSetDefaults.TableNameChs = item.ParentNode == null ? "" : item.ParentNode.Attributes["value"].Value;
                OSetDefaults.ColumnName = item.Attributes["colum"].Value;
                OSetDefaults.ColumnNameChs = item.Attributes["name"].Value;
                OSetDefaults.DataType = item.Attributes["type"].Value;
                OSetDefaults.DefaultValue = item.Attributes["value"].Value;
                ArrSetDefaults.Add(OSetDefaults);
            }
            //加锁 防止 多人同时 登录时引发 Cache 对象已具有相同键值
            lock (Common.lockCacheSetDefault)
            {
                if (HttpContext.Cache[Common.GeCacheEnumByName("SetDefaults").ToString()] != null)
                    HttpContext.Cache.Remove(Common.GeCacheEnumByName("SetDefaults").ToString());
                HttpContext.Cache.Insert(Common.GeCacheEnumByName("SetDefaults").ToString(), ArrSetDefaults, null, DateTime.Now.AddDays(10), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        /// 创建XML文件
        /// </summary>
        public XmlDocument CreateXML()
        {
            //功能，新建配置文件，并配置表节点
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(Server.MapPath(XMLPath));
            }
            catch (System.IO.FileNotFoundException)
            {
                XmlNode declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "no");
                doc.AppendChild(declaration);
                XmlElement root = doc.CreateElement("WebSettings");
                doc.AppendChild(root);
                doc.Save(Server.MapPath(XMLPath));
            }
            return doc;
        }

        //根据名称获取 类
        public object getTableClass(String s)
        {
            object retobj = null;
            Type _Type = Common.GetTypeByClassName(s);
            if (_Type != null)
                retobj = Activator.CreateInstance(_Type);
            return retobj;
        }

        //获取所有可设置默认值的表
        public List<SelectListItem> getXMLDefaultTables()
        {
            List<SelectListItem> ArrSelectList = new List<SelectListItem>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath(XMLPath));
                XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/DefaultTables");
                if (xmlTableNode != null)
                {
                    if (xmlTableNode.HasChildNodes)
                    {
                        XmlNodeList xmlTableNodeList = xmlTableNode.SelectNodes("table");
                        if (xmlTableNodeList != null)
                        {
                            SelectListItem OSelectListItem = new SelectListItem();
                            OSelectListItem.Text = "---请选择---";
                            OSelectListItem.Value = "";
                            ArrSelectList.Add(OSelectListItem);
                            if (xmlTableNodeList.Count > 0)
                            {
                                foreach (XmlNode item in xmlTableNodeList)
                                {
                                    XmlElement XeTableNode = (XmlElement)item;
                                    OSelectListItem = new SelectListItem();
                                    OSelectListItem.Text = XeTableNode.GetAttribute("value");
                                    OSelectListItem.Value = XeTableNode.GetAttribute("name");
                                    ArrSelectList.Add(OSelectListItem);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                ArrSelectList = new List<SelectListItem>();
            }
            return ArrSelectList;
        }

        //获取所有可设置默认值的表
        public ActionResult getXMLDefaultTablesData(string q="")
        {
            List<SelectListItem> ArrSelectList = new List<SelectListItem>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Server.MapPath(XMLPath));
                XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/DefaultTables");
                if (xmlTableNode != null)
                {
                    if (xmlTableNode.HasChildNodes)
                    {
                        XmlNodeList xmlTableNodeList = xmlTableNode.SelectNodes("table");
                        if (xmlTableNodeList != null)
                        {
                            SelectListItem OSelectListItem = new SelectListItem();
                            if (q != "combogrid")
                            {
                                OSelectListItem.Text = "---请选择---";
                                OSelectListItem.Value = "";
                                ArrSelectList.Add(OSelectListItem);
                            }
                            if (xmlTableNodeList.Count > 0)
                            {
                                foreach (XmlNode item in xmlTableNodeList)
                                {
                                    XmlElement XeTableNode = (XmlElement)item;
                                    OSelectListItem = new SelectListItem();
                                    OSelectListItem.Text = XeTableNode.GetAttribute("value");
                                    OSelectListItem.Value = XeTableNode.GetAttribute("name");
                                    ArrSelectList.Add(OSelectListItem);
                                }
                            }
                        }
                    }
                }

                var rows = ArrSelectList.Select(n => new { Value = n.Value, Text = n.Text });
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ArrSelectList = new List<SelectListItem>();
            } 
            return Json(ArrSelectList, JsonRequestBehavior.AllowGet);
        }

        //根据表名获取所有列
        public List<SelectListItem> GetAllColumnByTableName(string TableName = "", string q="")
        {
            List<SelectListItem> ArrSelectList = new List<SelectListItem>();

            SelectListItem OSelectListItem = new SelectListItem();
            if (q != "1")
            {
                OSelectListItem.Text = "---请选择---";
                OSelectListItem.Value = "";
                ArrSelectList.Add(OSelectListItem);
            }

            Object Obj_Table = getTableClass(TableName);
            if (Obj_Table != null)
            {
                System.Reflection.PropertyInfo[] PropertyInfos = Obj_Table.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    var FullName = fi.PropertyType.FullName;
                    if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var Arguments = fi.PropertyType.GetGenericArguments();
                        FullName = Arguments[0].FullName;
                    }
                    if (FullName.ToLower().IndexOf("AirOut.Web.models") >= 0)
                        continue;
                    //获取display属性操作对象
                    DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
                    if (disAttr != null)
                    {
                        OSelectListItem = new SelectListItem();
                        OSelectListItem.Text = disAttr.Name;
                        OSelectListItem.Value = fi.Name;//.Replace("_", "-") + "_" + fi.PropertyType.Name;
                        ArrSelectList.Add(OSelectListItem);
                    }
                    else
                    {
                        OSelectListItem = new SelectListItem();
                        OSelectListItem.Text = fi.Name;
                        OSelectListItem.Value = fi.Name;//.Replace("_", "-") + "_" + fi.PropertyType.Name;
                        ArrSelectList.Add(OSelectListItem);
                    }
                }
            }
            return ArrSelectList;
        }

        //根据表名和字段名 获取数据类型
        public string GetDataTypeByTable_Column(string TableName = "", string ColumnName = "")
        {
            string ret = "";
            Object Obj_Table = getTableClass(TableName);
            if (Obj_Table != null)
            {
                System.Reflection.PropertyInfo[] PropertyInfos = Obj_Table.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                //遍历该model实体的所有字段
                foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
                {
                    if (fi.Name.ToLower() == ColumnName.ToLower())
                    {
                        ret = fi.PropertyType.Name;
                        if (fi.PropertyType.IsGenericType && fi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var Arguments = fi.PropertyType.GetGenericArguments();
                            ret = Arguments[0].Name;
                        }
                        break;
                    }
                }
            }
            return ret;
        }

        //为类设置 默认值
        public void SetDefaultValueToModel(string TableName = "", Object TableClass = null)
        {
            if (TableName == "")
                return;
            if (TableClass == null)
                return;
            List<SetDefaults> list = getAllSetDefaultsByTable(TableName);
            if (!list.Any())
                return;
            System.Reflection.PropertyInfo[] PropertyInfos = TableClass.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            //遍历该model实体的所有字段
            foreach (System.Reflection.PropertyInfo fi in PropertyInfos)
            {
                //获取字段名，用于查找该字段对应的display数据，来源List<ColumValue>
                String FiledName = fi.Name;
                object s = fi.GetValue(TableClass, null);
                //获取display属性操作对象
                DisplayAttribute disAttr = (DisplayAttribute)fi.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

                foreach (var item in list)
                {
                    if (item.ColumnName == FiledName)
                    {
                        switch (item.DataType.ToLower())
                        {
                            case "int":
                                int Dftint = 0;
                                if (int.TryParse(item.DefaultValue, out Dftint))
                                {
                                    fi.SetValue(TableClass, Dftint, null);
                                }
                                break;
                            case "decimal":
                                decimal Dftdecimal = 0;
                                if (decimal.TryParse(item.DefaultValue, out Dftdecimal))
                                {
                                    fi.SetValue(TableClass, Dftdecimal, null);
                                }
                                break;
                            case "string":
                                fi.SetValue(TableClass, item.DefaultValue, null);
                                break;
                            case "datetime":
                                int TDatetime = 0;
                                if (int.TryParse(item.DefaultValue, out TDatetime))
                                {
                                    fi.SetValue(TableClass, DateTime.Now.AddDays(TDatetime), null);
                                }
                                else
                                {
                                    DateTime DftDateTime = new DateTime();
                                    if (DateTime.TryParse(item.DefaultValue, out DftDateTime))
                                    {
                                        fi.SetValue(TableClass, DftDateTime, null);
                                    }
                                }
                                break;
                            case "bool":
                                bool Dftbool = false;
                                if (bool.TryParse(item.DefaultValue, out Dftbool))
                                {
                                    fi.SetValue(TableClass, Dftbool, null);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }


            }

        }

        //根据表名和字段名 获取XML节点 并返回为 SetDefaults
        public SetDefaults getSetDefaultsByTable_Column(string TableName = "", string ColumnName = "")
        {
            SetDefaults OSetDefaults = new SetDefaults();
            if (TableName == "" || ColumnName == "")
            {
                OSetDefaults = null;
            }
            else
            {
                try
                {
                    OSetDefaults = null;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath(XMLPath));
                    XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/table[@name='" + TableName + "']");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNode xmlAutoSetNode = xmlTableNode.SelectSingleNode("AutoSet[@colum='" + ColumnName + "']");
                            if (xmlAutoSetNode != null)
                            {
                                OSetDefaults = new SetDefaults();
                                OSetDefaults.TableName = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["name"].Value;
                                OSetDefaults.TableNameChs = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["value"].Value;
                                OSetDefaults.ColumnName = xmlAutoSetNode.Attributes["colum"].Value;
                                OSetDefaults.ColumnNameChs = xmlAutoSetNode.Attributes["name"].Value;
                                OSetDefaults.DataType = xmlAutoSetNode.Attributes["type"].Value;
                                OSetDefaults.DefaultValue = xmlAutoSetNode.Attributes["value"].Value;
                            }
                        }
                    }
                }
                catch
                {
                    OSetDefaults = null;
                }
            }
            return OSetDefaults;
        }

        //根据表名 获取表的所有默认值 并返回为 List<SetDefaults>
        public List<SetDefaults> getAllSetDefaultsByTable(string TableName = "")
        {
            List<SetDefaults> ArrSetDefaults = new List<SetDefaults>();
            SetDefaults OSetDefaults = new SetDefaults();
            if (TableName == "")
            {
                ArrSetDefaults = new List<SetDefaults>();
            }
            else
            {
                try
                {
                    OSetDefaults = null;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath(XMLPath));
                    XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/table[@name='" + TableName + "']");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNodeList xmlAutoSetNodes = xmlTableNode.SelectNodes("AutoSet");
                            if (xmlAutoSetNodes != null)
                            {
                                foreach (XmlNode xmlAutoSetNode in xmlAutoSetNodes)
                                {
                                    OSetDefaults = new SetDefaults();
                                    OSetDefaults.TableName = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["name"].Value;
                                    OSetDefaults.TableNameChs = xmlAutoSetNode.ParentNode == null ? "" : xmlAutoSetNode.ParentNode.Attributes["value"].Value;
                                    OSetDefaults.ColumnName = xmlAutoSetNode.Attributes["colum"].Value;
                                    OSetDefaults.ColumnNameChs = xmlAutoSetNode.Attributes["name"].Value;
                                    OSetDefaults.DataType = xmlAutoSetNode.Attributes["type"].Value;
                                    OSetDefaults.DefaultValue = xmlAutoSetNode.Attributes["value"].Value;
                                    ArrSetDefaults.Add(OSetDefaults);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ArrSetDefaults = new List<SetDefaults>();
                }
            }
            return ArrSetDefaults;
        }

        //根据表名和字段名 删除Xml节点
        public List<string> DeleteByTable_Column(string TableName = "", string ColumnName = "")
        {
            List<string> ErrorList = new List<string>();
            if (TableName != "" && ColumnName != "")
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath(XMLPath));
                    XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/table[@name='" + TableName + "']");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNode xmlAutoSetNode = xmlTableNode.SelectSingleNode("AutoSet[@colum='" + ColumnName + "']");
                            if (xmlAutoSetNode != null)
                            {
                                xmlTableNode.RemoveChild(xmlAutoSetNode);
                                doc.Save(Server.MapPath(XMLPath));
                            }
                            else
                                ErrorList.Add("表中的默认值节点不存在");
                        }
                        else
                            ErrorList.Add("表中的默认值节点不存在");
                    }
                    else
                        ErrorList.Add("表不存在");
                }
                catch (Exception e)
                {
                    ErrorList.Add(e.Message);
                }
            }
            return ErrorList;
        }

        //根据表名和字段名 更新Xml节点
        public List<string> UpdateXMLBySetDefaults(SetDefaults OSetDefaults)
        {
            List<string> ErrorList = new List<string>();
            if (OSetDefaults != null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath(XMLPath));
                    XmlNode xmlTableNode = doc.SelectSingleNode("WebSettings/table[@name='" + OSetDefaults.TableName + "']");
                    if (xmlTableNode != null)
                    {
                        if (xmlTableNode.HasChildNodes)
                        {
                            XmlNode xmlAutoSetNode = xmlTableNode.SelectSingleNode("AutoSet[@colum='" + OSetDefaults.ColumnName + "']");
                            if (xmlAutoSetNode != null)
                            {
                                //XmlElement XeTableNode=(XmlElement)xmlTableNode;
                                XmlElement XeAutoSetNode = (XmlElement)xmlAutoSetNode;
                                XeAutoSetNode.SetAttribute("value", OSetDefaults.DefaultValue);
                                doc.Save(Server.MapPath(XMLPath));
                                Set_UpdateXMLCache();
                            }
                            else
                                ErrorList.Add("表中的默认值节点不存在");
                        }
                        else
                            ErrorList.Add("表中的默认值节点不存在");
                    }
                    else
                        ErrorList.Add("表不存在");
                }
                catch (Exception e)
                {
                    ErrorList.Add(e.Message);
                }
            }
            return ErrorList;
        }

        //根据表名和字段名 添加Xml节点
        public List<string> AddXMLBySetDefaults(SetDefaults OSetDefaults)
        {
            List<string> ErrorList = new List<string>();
            if (OSetDefaults != null)
            {
                try
                {
                    XmlDocument XMLdoc = new XmlDocument();
                    XMLdoc.Load(Server.MapPath(XMLPath));
                    XmlNode root = XMLdoc.SelectSingleNode("WebSettings");
                    XmlNode xmlTableNode = root.SelectSingleNode("table[@name='" + OSetDefaults.TableName + "']");

                    XmlElement XeRoot;
                    XmlElement XeTableNode;
                    XmlElement XeAutoSetNode;

                    if (root != null)
                    {
                        XeRoot = (XmlElement)root;
                    }
                    else
                    {
                        XeRoot = XMLdoc.CreateElement("WebSettings");
                    }
                    if (xmlTableNode != null)
                    {
                        XeTableNode = (XmlElement)xmlTableNode;

                        XmlNode xmlAutoSetNode = xmlTableNode.SelectSingleNode("AutoSet[@colum='" + OSetDefaults.ColumnName + "']");
                        if (xmlAutoSetNode != null)
                        {
                            XeAutoSetNode = (XmlElement)xmlAutoSetNode;
                        }
                        else
                        {
                            XeAutoSetNode = XMLdoc.CreateElement("AutoSet");
                        }
                        XeAutoSetNode.SetAttribute("colum", OSetDefaults.ColumnName);
                        XeAutoSetNode.SetAttribute("value", OSetDefaults.DefaultValue);
                        XeAutoSetNode.SetAttribute("name", OSetDefaults.ColumnNameChs);
                        XeAutoSetNode.SetAttribute("type", OSetDefaults.DataType);

                        if (xmlAutoSetNode == null)
                            xmlTableNode.AppendChild(XeAutoSetNode);
                    }
                    else
                    {
                        XeTableNode = XMLdoc.CreateElement("table");
                        XeTableNode.SetAttribute("name", OSetDefaults.TableName);
                        XeTableNode.SetAttribute("value", OSetDefaults.TableNameChs);

                        XeAutoSetNode = XMLdoc.CreateElement("AutoSet");
                        XeAutoSetNode.SetAttribute("colum", OSetDefaults.ColumnName);
                        XeAutoSetNode.SetAttribute("value", OSetDefaults.DefaultValue);
                        XeAutoSetNode.SetAttribute("name", OSetDefaults.ColumnNameChs);
                        XeAutoSetNode.SetAttribute("type", OSetDefaults.DataType);

                        XeTableNode.AppendChild(XeAutoSetNode);

                        XeRoot.AppendChild(XeTableNode);

                        if (root == null)
                            XMLdoc.AppendChild(XeRoot);
                    }
                    XMLdoc.Save(Server.MapPath(XMLPath));
                    Set_UpdateXMLCache();
                }
                catch (Exception e)
                {
                    ErrorList.Add(e.Message);
                }
            }
            return ErrorList;
        }

        #endregion

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
