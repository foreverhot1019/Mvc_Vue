using System;
using Newtonsoft.Json;
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
using System.Text;
using System.IO;

namespace AirOut.Web.Controllers
{
    public class PicturesController : BaseController//Controller
    {
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Picture>, Repository<Picture>>();
        //container.RegisterType<IPictureService, PictureService>();
        //private WebdbContext db = new WebdbContext();

        private readonly IPictureService _pictureService;
        private readonly IWarehouse_receiptService _warehouse_receiptService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ICustomsInspectionService _customsInspectionService;
        private readonly IOPS_M_OrderService _oPS_M_OrderService;
        private readonly IOPS_H_OrderService _oPS_H_OrderService;
        //验证权限的名称
        private string ControllerQXName = "/Pictures";

        public PicturesController(IPictureService pictureService, IWarehouse_receiptService warehouse_receiptService, ICustomsInspectionService customsInspectionService
            , IOPS_M_OrderService oPS_M_OrderService, IOPS_H_OrderService oPS_H_OrderService, IUnitOfWorkAsync unitOfWork)
        {
            _pictureService = pictureService;
            _warehouse_receiptService = warehouse_receiptService;
            _customsInspectionService = customsInspectionService;
            _oPS_M_OrderService = oPS_M_OrderService;
            _oPS_H_OrderService = oPS_H_OrderService;
            _unitOfWork = unitOfWork;
        }

        // GET: Pictures/Index
        public ActionResult Index()
        {
            //var picture  = _pictureService.Queryable().AsQueryable();
            //return View(picture  );
			return View();
        }

        // Get :Pictures/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
            var picture = _pictureService.Query(new PictureQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).SelectPage(page, rows, out totalCount);
            var datarows = picture.Select(n => new
            {
Id = n.Id, 
Code = n.Code, 
Status = n.Status, 
Type = n.Type, 
Adress = n.Address, 
OperatingPoint = n.OperatingPoint, 
Remark = n.Remark, 
ADDWHO = n.ADDWHO, 
ADDTS = n.ADDTS, 
EDITWHO = n.EDITWHO, 
                EDITTS = n.EDITTS
            }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(PictureChangeViewModel picture)
        {
            if (picture.updated != null)
            {
                #region

                var ControllActinMsg = "编辑";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Edit", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var updated in picture.updated)
                {
                    _pictureService.Update(updated);
                }
            }
            if (picture.deleted != null)
            {
                #region

                var ControllActinMsg = "删除";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Delete", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var deleted in picture.deleted)
                {
                    _pictureService.Delete(deleted);
                }
            }
            if (picture.inserted != null)
            {
                #region

                var ControllActinMsg = "创建";
                bool IsHaveQx = Common.ModelIsHaveQX(ControllerQXName, "Create", ControllActinMsg);
                if (!IsHaveQx)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { Success = false, ErrMsg = "您没有权限" + ControllActinMsg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Common.ExitToNoQx("您没有权限" + ControllActinMsg);
                        return null;
                    }
                }

                #endregion

                foreach (var inserted in picture.inserted)
                {
                    _pictureService.Insert(inserted);
                }
            }
            //如果需要 Ajax防止重复提交，需要手动 将表单唯一值 返回（前台会自动做处理）
            string ActionGuidName = ViewData["ActionGuidName"] == null ? "" : ViewData["ActionGuidName"].ToString();
            string ActionGuid = ViewData[ActionGuidName] == null ? "" : ViewData[ActionGuidName].ToString();
            try
            {
                if ((picture.updated != null && picture.updated.Any()) || 
				(picture.deleted != null && picture.deleted.Any()) || 
				(picture.inserted != null && picture.inserted.Any()))
                {
                    _unitOfWork.SaveChanges();
                    //自动更新 缓存
                    if (IsAutoResetCache)
						AutoResetCache(picture);
                    return Json(new { Success = true, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Success = false, ErrMsg = "没有任何需要更新数据库的操作", ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.AddError(ex, NotificationTag.Sys, "系统异常");
                string ErrMSg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMSg, ActionGuidName = ActionGuidName, ActionGuid = ActionGuid }, JsonRequestBehavior.AllowGet);
            }
        }
		       
        // GET: Pictures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = _pictureService.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // GET: Pictures/Create
        public ActionResult Create()
        {
            Picture picture = new Picture();
            //set default value
            return View(picture);
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Status,Type,Adress,OperatingPoint,Remark,ADDWHO,ADDTS,EDITWHO,EDITTS")] Picture picture)
        {
            if (ModelState.IsValid)
            {
				_pictureService.Insert(picture);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Picture record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(picture);
        }

        // GET: Pictures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = _pictureService.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Status,Type,Adress,OperatingPoint,Remark,ADDWHO,ADDTS,EDITWHO,EDITTS")] Picture picture)
        {
            if (ModelState.IsValid)
            {
				picture.ObjectState = ObjectState.Modified;
				_pictureService.Update(picture);
				_unitOfWork.SaveChanges();
				if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Picture record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { Success = false, ErrMsg = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(picture);
        }

        // GET: Pictures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = _pictureService.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = _pictureService.Find(id);
            _pictureService.Delete(picture);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Picture record");
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            Picture picture = _pictureService.Find(id);
            _pictureService.Delete(picture);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            //DisplaySuccessMessage("Has delete a Picture record");
            return Json(new { Success = false, ErrMsg = "删除错误" }, JsonRequestBehavior.AllowGet);
        }

		//导出Excel
		[HttpPost]
        public ActionResult ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
        {
            var fileName = "picture_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var stream = _pictureService.ExportExcel(filterRules, sort, order);
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteEmptyUpload(int ID = 0, string Talbe = "", string type = "")
        {
            try
            {
                var warehouse_receipt = new Warehouse_receipt();
                var pic = new Picture();
                switch (Talbe)
                {
                    case "Warehouse_receipt"://仓库接单
                        warehouse_receipt = _warehouse_receiptService.Find(ID);
                        if (warehouse_receipt == null)
                        {
                            return Json(new { Success = false, ErrMsg = "该订单未保存，请先保存订单在导入异常图片！" }, JsonRequestBehavior.AllowGet);
                        }
                        if (type == "DamageUpload")
                        {
                            var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Damaged && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                            if (piclist != null && piclist.Count() > 0)
                            {
                                foreach (var item in piclist)
                                {
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                    warehouse_receipt.Is_DamageUpload = false;
                                    _warehouse_receiptService.Update(warehouse_receipt);
                                }
                            }
                            else
                            {
                                return Json(new { Success = false, ErrMsg = "该订单未保存，没有导入图片！" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (type == "DeliveryUpload")
                        {
                            var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Dampness && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                            if (piclist != null && piclist.Count() > 0)
                            {
                                foreach (var item in piclist)
                                {
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                    warehouse_receipt.Is_DamageUpload = false;
                                    _warehouse_receiptService.Update(warehouse_receipt);
                                }
                            }
                            else
                            {
                                return Json(new { Success = false, ErrMsg = "该订单未保存，没有导入图片！" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (type == "EntryUpload")
                        {
                            var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Entry && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                            if (piclist != null && piclist.Count() > 0)
                            {
                                foreach (var item in piclist)
                                {
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                }
                                warehouse_receipt.Is_DamageUpload = false;
                                _warehouse_receiptService.Update(warehouse_receipt);
                            }
                            else
                            {
                                return Json(new { Success = false, ErrMsg = "该订单未保存，没有导入图片！" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        break;
                }
                _unitOfWork.SaveChanges();
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteUpload(int ID = 0, string Talbe = "", string type = "")
        {
            try
            {
                 var warehouse_receipt = new Warehouse_receipt();
                 var pic = new Picture();
                 switch (Talbe)
                 {
                     case "Warehouse_receipt"://仓库接单
                         warehouse_receipt = _warehouse_receiptService.Find(ID);
                            if (warehouse_receipt == null)
                            {
                                return Json(new { Success = false, ErrMsg = "该订单未保存，请先保存订单在导入异常图片！" }, JsonRequestBehavior.AllowGet);
                            }
                            if (type == "DamageUpload")
                            {
                                var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Damaged && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                                if (piclist != null && piclist.Count() > 0 )
                                {
                                    var item = piclist.FirstOrDefault();
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                    if (piclist.Count() == 1)
                                    {
                                        warehouse_receipt.Is_DamageUpload = false;
                                        _warehouse_receiptService.Update(warehouse_receipt);
                                    }
                                }
                            }
                            else if (type == "DeliveryUpload")
                            {
                                var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Dampness && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                                if (piclist != null && piclist.Count() > 0)
                                {
                                    var item = piclist.FirstOrDefault();
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                    if (piclist.Count() == 1)
                                    {
                                        warehouse_receipt.Is_DamageUpload = false;
                                        _warehouse_receiptService.Update(warehouse_receipt);
                                    }
                                }
                            }
                            else if (type == "EntryUpload")
                            {
                                var piclist = _pictureService.Queryable().Where(x => x.Code.Equals(warehouse_receipt.Entry_Id) && x.Type == AirOutEnumType.PictureTypeEnum.Entry && x.Status == AirOutEnumType.UseStatusIsOrNoEnum.Enable).OrderByDescending(x => x.Id);
                                if (piclist != null && piclist.Count() > 0)
                                {
                                    var item = piclist.FirstOrDefault();
                                    item.Status = AirOutEnumType.UseStatusIsOrNoEnum.draft;
                                    _pictureService.Update(item);
                                    if (piclist.Count() == 1)
                                    {
                                        warehouse_receipt.Is_DamageUpload = false;
                                        _warehouse_receiptService.Update(warehouse_receipt);
                                    }
                                }
                            }
                         break;
                 }
                _unitOfWork.SaveChanges();
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string ErrMsg = Common.GetExceptionMsg(ex);
                return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase Filedata, int ID = 0, string Talbe = "", string type = "")
        {
            if (Filedata != null)
            {
                try
                {
                    // 如果没有上传文件
                    if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                    {
                        return this.HttpNotFound();
                    }

                    var warehouse_receipt = new Warehouse_receipt();
                    var customsInspection = new CustomsInspection();
                    var ops_m_order = new OPS_M_Order();
                    var ops_h_order = new OPS_H_Order();
                    var pic = new Picture();

                    #region 获取上传图片地址

                    string uploadfilename = System.IO.Path.GetFileName(Filedata.FileName);
                    string fileExtension = Path.GetExtension(uploadfilename);         //文件扩展名
                    string fileName = Path.GetFileNameWithoutExtension(uploadfilename);
                    string folder = System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"] == null ? "/FileUpLoad/" : System.Configuration.ConfigurationManager.AppSettings["FileUpLoadPath"].ToString();
                    var folder1 = folder + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                    var folder2 = "/FileUpLoad/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                    folder1 = Server.MapPath(folder1);
                    string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");//获取时间
                    string newFileName = string.Format("{0}_{1}", fileName, Common.GenerateRandomCode(3)) + fileExtension;//重组成新的文件名

                    if (!Directory.Exists(folder1))
                    {
                        Directory.CreateDirectory(folder1);
                    }

                    string virtualPath = string.Format("{0}{1}", folder1, newFileName);
                    string virtualPath2 = string.Format("{0}{1}", folder2, newFileName);

                    #endregion 
                    
                    #region 保存上传表的状态
                    switch (Talbe)
                    {
                        #region 仓库接单 破损/交换/进仓  图片上传
                        case "Warehouse_receipt":
                            warehouse_receipt = _warehouse_receiptService.Find(ID);
                            if (warehouse_receipt == null)
                            {
                                return Json(new { Success = false, ErrMsg = "该仓库接单未保存，请先保存仓库接单在导入异常图片！" }, JsonRequestBehavior.AllowGet);
                            }
                            pic.Code = warehouse_receipt.Warehouse_Id;
                            pic.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                            if (type == "DamageUpload")
                            {
                                pic.Type = AirOutEnumType.PictureTypeEnum.Damaged;
                                warehouse_receipt.Is_DamageUpload = true;
                            }
                            else if (type == "DeliveryUpload")
                            {
                                pic.Type = AirOutEnumType.PictureTypeEnum.Dampness;
                                warehouse_receipt.Is_DeliveryUpload = true;
                            }
                            else if (type == "EntryUpload")
                            {
                                pic.Type = AirOutEnumType.PictureTypeEnum.Entry;
                                warehouse_receipt.Is_EntryUpload = true;
                            }
                            _warehouse_receiptService.Update(warehouse_receipt);
                            break;
                            #endregion
                        #region 报关报检  附档上传
                        case "CustomsInspections":
                            customsInspection = _customsInspectionService.Find(ID);
                            if (customsInspection == null) 
                            {
                                return Json(new { Success = false, ErrMsg = "该报关报检未保存，请先保存报关报检信息在导入图片！" }, JsonRequestBehavior.AllowGet);
                            }
                            pic.Code = customsInspection.Operation_ID;
                            pic.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                            pic.Type = AirOutEnumType.PictureTypeEnum.Fileupload;
                            pic.Remark = uploadfilename;

                            
                            var CI = _customsInspectionService.Queryable().Where(x => x.Operation_ID == customsInspection.Operation_ID).ToList();
                            if (CI != null)
                            {
                                foreach (var item in CI)
                                {
                                    item.Fileupload = virtualPath2;
                                    _customsInspectionService.Update(item);
                                }
                            }
                            break;
                        #endregion
                        #region 总单附档文件上传
                        case "OPS_M_Order":
                            ops_m_order = _oPS_M_OrderService.Find(ID);
                            if (ops_m_order == null)
                            {
                                return Json(new { Success = false, ErrMsg = "该总单未保存，请先保存订单在导入报关报检附档文件！" }, JsonRequestBehavior.AllowGet);
                            }
                            if (ops_m_order.MBL == null || ops_m_order.MBL == "")
                            {
                                return Json(new { Success = false, ErrMsg = "该总单号不存在，请先输入并保存总单号在导入总单附档文件！" }, JsonRequestBehavior.AllowGet);
                            }
                            pic.Code = ops_m_order.MBL + ID.ToString();
                            pic.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                            pic.Type = AirOutEnumType.PictureTypeEnum.Fileupload_MBL;
                            pic.Remark = uploadfilename;

                            ops_m_order.File_M = virtualPath2;
                            _oPS_M_OrderService.Update(ops_m_order);

                            break;
                        #endregion
                        #region 分单附档文件上传
                        case "OPS_H_Order":
                            ops_h_order = _oPS_H_OrderService.Find(ID);
                            if (ops_h_order == null)
                            {
                                return Json(new { Success = false, ErrMsg = "该分单未保存，请先保存订单在导入报关报检附档文件！" }, JsonRequestBehavior.AllowGet);
                            }
                            if (string.IsNullOrEmpty(ops_h_order.HBL))
                            {
                                return Json(new { Success = false, ErrMsg = "该分单号不存在，请先输入并保存分单号在导入分单附档文件！" }, JsonRequestBehavior.AllowGet);
                            }
                            pic.Code = ops_h_order.HBL + ID.ToString();
                            pic.Status = AirOutEnumType.UseStatusIsOrNoEnum.Enable;
                            pic.Type = AirOutEnumType.PictureTypeEnum.Fileupload_HBL;
                            pic.Remark = uploadfilename;

                            //_oPS_H_OrderService.Update(ops_h_order);
                            break;
                        #endregion
                    }
                    #endregion

                    //var pic = _pictureService.Find(ID);
                    //if (pic == null)
                    //{
                    //    return Json(new { Success = false, ErrMsg = "该订单未保存，请先保存订单在导入异常图片！" }, JsonRequestBehavior.AllowGet);
                    //}

                    ControllerContext.HttpContext.Request.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.Charset = "UTF-8";

                    #region 保存图片

                    Filedata.SaveAs(virtualPath);

                    pic.Address = virtualPath2;//保存订单地址
                    //_pictureService.Update(pic);
                    _pictureService.Insert(pic);

                    _unitOfWork.SaveChanges();
                    return Json(new { Success = true, filename = virtualPath2, newFileName = newFileName }, JsonRequestBehavior.AllowGet);
                    #endregion

                    // 文件上传后的保存路径
                    //string filePath = Server.MapPath("~/UploadFiles/");
                    //DirectoryUtil.AssertDirExist(filePath);

                    //string fileName = Path.GetFileName(fileData.FileName);      //原始文件名称
                    //string fileExtension = Path.GetExtension(fileName);         //文件扩展名
                    //string saveName = Guid.NewGuid().ToString() + fileExtension; //保存文件名称

                    //Picture info = new Picture();
                    //info.FileData = ReadFileBytes(fileData);
                    //if (info.FileData != null)
                    //{
                    //    info.FileSize = info.FileData.Length;
                    //}
                    //info.Category = folder;
                    //info.FileName = fileName;
                    //info.FileExtend = fileExtension;
                    //info.AttachmentGUID = guid;
                    //info.AddTime = DateTime.Now;
                    //info.Editor = CurrentUser.Name;//登录人
                    ////info.Owner_ID = OwerId;//所属主表记录ID

                    //CommonResult result = BLLFactory<FileUpload>.Instance.Upload(info);
                    //if (!result.Success)
                    //{
                    //    LogTextHelper.Error("上传文件失败:" + result.ErrorMessage);
                    //}
                    //return Content(result.Success.ToString());
                }
                catch (Exception e)
                {
                    string ErrMsg = Common.GetExceptionMsg(e);
                    return Json(new { Success = false, ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, ErrMsg = "没有选中图片" }, JsonRequestBehavior.AllowGet);
            }
        }

        private byte[] ReadFileBytes(HttpPostedFileBase fileData)
        {
            byte[] data;
            using (Stream inputStream = fileData.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        //获取客商状态枚举
        public ActionResult GetPictureTypeEnum()
        {
            var ArrEnum = Common.GetEnumToDic("AuditStatusEnum", "AirOut.Web.Models.PictureTypeEnum");
            var list = ArrEnum.Select(n => new { ID = n.Value, TEXT = n.DisplayName, Key = n.Key });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText; 
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }
    }
}