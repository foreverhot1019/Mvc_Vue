


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
using TMI.Web.Models;
using TMI.Web.Services;
using TMI.Web.Repositories;
using TMI.Web.Extensions;


namespace TMI.Web.Controllers
{
    public class NotificationsController : Controller
    {
        
        //Please RegisterType UnityConfig.cs
        //container.RegisterType<IRepositoryAsync<Notification>, Repository<Notification>>();
        //container.RegisterType<INotificationService, NotificationService>();
        
        //private WebdbContext db = new WebdbContext();
        private readonly INotificationService  _notificationService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public NotificationsController (INotificationService  notificationService, IUnitOfWorkAsync unitOfWork)
        {
            _notificationService  = notificationService;
            _unitOfWork = unitOfWork;
        }

        // GET: Notifications/Index
        public ActionResult Index()
        {
            
            //var notifications  = _notificationService.Queryable().AsQueryable();
            //return View(notifications  );
			return View();
        }

        // Get :Notifications/PageList
        // For Index View Boostrap-Table load  data 
        [HttpGet]
        public ActionResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
            int totalCount = 0;
            //int pagenum = offset / limit +1;
                        var notifications  = _notificationService.Query(new NotificationQuery().Withfilter(filters)).OrderBy(n=>n.OrderBy(sort,order)).SelectPage(page, rows, out totalCount);
                        var datarows = notifications .Select(  n => new {  Id = n.Id , Name = n.Name , Description = n.Description , Type = n.Type , Sender = n.Sender , Receiver = n.Receiver , Schedule = n.Schedule , Disabled = n.Disabled , AuthUser = n.AuthUser , AuthPassword = n.AuthPassword , Host = n.Host }).ToList();
            var pagelist = new { total = totalCount, rows = datarows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		public ActionResult SaveData(NotificationChangeViewModel notifications)
        {
            if (notifications.updated != null)
            {
                foreach (var updated in notifications.updated)
                {
                    _notificationService.Update(updated);
                }
            }
            if (notifications.deleted != null)
            {
                foreach (var deleted in notifications.deleted)
                {
                    _notificationService.Delete(deleted);
                }
            }
            if (notifications.inserted != null)
            {
                foreach (var inserted in notifications.inserted)
                {
                    _notificationService.Insert(inserted);
                }
            }
            _unitOfWork.SaveChanges();

            return Json(new {Success=true}, JsonRequestBehavior.AllowGet);
        }

		
				public ActionResult GetNotifications()
        {
            var notificationRepository = _unitOfWork.Repository<Notification>();
            var data = notificationRepository.Queryable().ToList();
            var rows = data.Select(n => new { Id = n.Id, Name = n.Name });
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
		
       
        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = _notificationService.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }
        

        // GET: Notifications/Create
        public ActionResult Create()
        {
            Notification notification = new Notification();
            //set default value
            return View(notification);
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Messages,Id,Name,Description,Type,Sender,Receiver,Schedule,Disabled,AuthUser,AuthPassword,Host,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                             notification.ObjectState = ObjectState.Added;   
                                foreach (var item in notification.Messages)
                {
					item.NotificationId = notification.Id ;
                    item.ObjectState = ObjectState.Added;
                }
                                _notificationService.InsertOrUpdateGraph(notification);
                            _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has append a Notification record");
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = _notificationService.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Messages,Id,Name,Description,Type,Sender,Receiver,Schedule,Disabled,AuthUser,AuthPassword,Host,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.ObjectState = ObjectState.Modified;
                                                foreach (var item in notification.Messages)
                {
					item.NotificationId = notification.Id ;
                    //set ObjectState with conditions
                    if(item.Id <= 0)
                        item.ObjectState = ObjectState.Added;
                    else
                        item.ObjectState = ObjectState.Modified;
                }
                      
                _notificationService.InsertOrUpdateGraph(notification);
                                
                _unitOfWork.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                DisplaySuccessMessage("Has update a Notification record");
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var modelStateErrors =String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
            DisplayErrorMessage();
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = _notificationService.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification =  _notificationService.Find(id);
             _notificationService.Delete(notification);
            _unitOfWork.SaveChanges();
           if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            DisplaySuccessMessage("Has delete a Notification record");
            return RedirectToAction("Index");
        }


        // Get Detail Row By Id For Edit
        // Get : Notifications/EditMessage/:id
        [HttpGet]
        public ActionResult EditMessage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var messageRepository = _unitOfWork.Repository<Message>();
            var message = messageRepository.Find(id);

                        var notificationRepository = _unitOfWork.Repository<Notification>();             
            
            if (message == null)
            {
                            ViewBag.NotificationId = new SelectList(notificationRepository.Queryable(), "Id", "Name" );
                            
                //return HttpNotFound();
                return PartialView("_MessageEditForm", new Message());
            }
            else
            {
                            ViewBag.NotificationId = new SelectList(notificationRepository.Queryable(), "Id", "Name" , message.NotificationId );  
                             
            }
            return PartialView("_MessageEditForm",  message);

        }
        
        // Get Create Row By Id For Edit
        // Get : Notifications/CreateMessage
        [HttpGet]
        public ActionResult CreateMessage()
        {
                        var notificationRepository = _unitOfWork.Repository<Notification>();    
              ViewBag.NotificationId = new SelectList(notificationRepository.Queryable(), "Id", "Name" );
                      return PartialView("_MessageEditForm");

        }

        // Post Delete Detail Row By Id
        // Get : Notifications/DeleteMessage/:id
        [HttpPost,ActionName("DeleteMessage")]
        public ActionResult DeleteMessageConfirmed(int  id)
        {
            var messageRepository = _unitOfWork.Repository<Message>();
            messageRepository.Delete(id);
            _unitOfWork.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            DisplaySuccessMessage("Has delete a Order record");
            return RedirectToAction("Index");
        }

       

        // Get : Notifications/GetMessagesByNotificationId/:id
        [HttpGet]
        public ActionResult GetMessagesByNotificationId(int id)
        {
            var messages = _notificationService.GetMessagesByNotificationId(id);
            if (Request.IsAjaxRequest())
            {
                return Json(messages.Select( n => new { NotificationName = (n.Notification==null?"": n.Notification.Name) , Id = n.Id , Subject = n.Subject , Content = n.Content , Type = n.Type , NewDate = n.NewDate , IsSended = n.IsSended , SendDate = n.SendDate , NotificationId = n.NotificationId }),JsonRequestBehavior.AllowGet);
            }  
            return View(messages); 

        }
 

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //_unitOfWork.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
