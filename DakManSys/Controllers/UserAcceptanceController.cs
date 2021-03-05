using DakManSys.Entity;
using DakManSys.Security;
using DakManSys.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Controllers
{
    public class UserAcceptanceController : Controller
    {

        //
        // GET: /UserAcceptance/
        [CustomAuthorize(Roles = "User")]
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Admin,User")]
        public ActionResult Detail(string inwardNo,string flag)
        {
            try
            {
                ParentViewModel model = new ParentViewModel();
                using (var db = new DRSEntities())
                {

                    var query = (from m in db.Jct_Dak_Register
                                 join a in db.JCT_EMP_HOD on m.Hod_Code equals a.Hod_Code
                                 join b in db.Jct_Dak_Party_Header on m.Party_Code equals b.Party_Code
                                 join d in db.Jct_Dak_Party_Detail_Address on m.Party_Code equals d.Party_Code
                                 join c in db.Jct_Dak_Register_Cheque on m.Inward_No equals c.Inward_No into recieve
                                 where m.Inward_No == inwardNo
                                 from c in recieve.DefaultIfEmpty()
                                 select new DakManSys.ViewModel.ParentViewModel
                                 {
                                     Jct_Dak_Register = m,
                                     Jct_Dak_Register_Cheque = c,
                                     Jct_Dak_Party_Header = b,
                                     Jct_Dak_Party_Detail_Address = d,
                                     JCT_EMP_HOD = a
                                 });
                    db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                    foreach (var item in query)
                    {
                        model.Jct_Dak_Register = item.Jct_Dak_Register;
                        model.Jct_Dak_Register_Cheque = item.Jct_Dak_Register_Cheque;
                        model.Jct_Dak_Party_Header = item.Jct_Dak_Party_Header;
                        model.Jct_Dak_Party_Detail_Address = item.Jct_Dak_Party_Detail_Address;
                        model.JCT_EMP_HOD = item.JCT_EMP_HOD;
                    }
                    TempData["Model"] = model;

                }
                if (flag.Equals("report", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Json(new
                    {
                        redirectUrl = Url.Action("Details", "UserAcceptance"),
                        isRedirect = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Details", "UserAcceptance", null);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return Json(new { isRedirect = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [CustomAuthorize(Roles="User,Admin")]
        public ActionResult Details()
        {
            TempData.Keep("Model");
            ParentViewModel model = new ParentViewModel();
            model = (ParentViewModel)TempData["model"];
            
            model.displayDate = model.Jct_Dak_Register.Created_On.Value.ToShortDateString();
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "User")]
        public ActionResult UserAcceptSave(Dictionary<string, string> myDictionary)
        {
            using (DRSEntities db = new DRSEntities())
            {
                try
                {
                    DakManSys.ViewModel.GridViewModel model = new ViewModel.GridViewModel();
                    var key = myDictionary.Select(p => p.Value).First();
                    var value = key.Split(',');
                    string inwardno = value[0];
                    string remarks = value[1];
                    model.Jct_Dak_Register_Recieved.Inward_No = inwardno;
                    model.Jct_Dak_Register_Recieved.Remarks = remarks;
                    model.Jct_Dak_Register_Recieved.Received_Status = true;
                    model.Jct_Dak_Register_Recieved.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    model.Jct_Dak_Register_Recieved.Created_On = System.DateTime.Now;
                    model.Jct_Dak_Register_Recieved.Empcode = HttpContext.User.Identity.Name;
                    model.Jct_Dak_Register_Recieved.Created_Hostname = Environment.MachineName;

                    var current = db.Jct_Dak_Register_Recieved.Single(x => x.Inward_No == model.Jct_Dak_Register_Recieved.Inward_No);

                    if (current != null)
                    {
                        current.Ip_Address = model.Jct_Dak_Register_Recieved.Ip_Address;
                        current.Received_Status = model.Jct_Dak_Register_Recieved.Received_Status;
                        current.Remarks = model.Jct_Dak_Register_Recieved.Remarks;
                        current.Created_Hostname = model.Jct_Dak_Register_Recieved.Created_Hostname;
                        current.Created_On = model.Jct_Dak_Register_Recieved.Created_On;
                        current.Empcode = model.Jct_Dak_Register_Recieved.Empcode;
                        db.Entry(current).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    return Json("Fail" + ex.ToString(), JsonRequestBehavior.AllowGet);
                }
                
            }
            return Json("Success" , JsonRequestBehavior.AllowGet);
        }

         [CustomAuthorize(Roles = "User")]
        public ActionResult UserAcceptReplySave(Dictionary<string, string> myDictionary)
        {
            using (DRSEntities db = new DRSEntities())
            {
                try
                {
                    db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                    DakManSys.ViewModel.GridViewModel model = new ViewModel.GridViewModel();
                    var key = myDictionary.Select(p => p.Value).First();
                    var value = key.Split(',');
                    string inwardno = value[0];
                    string remarks = value[1];
                    string replyrefer = value[2];
                    DateTime replydate = Convert.ToDateTime(value[3]);
                    model.Jct_Dak_Register_Recieved.Inward_No = inwardno;
                    model.Jct_Dak_Register_Recieved.Remarks = remarks;
                    model.Jct_Dak_Register_Recieved.Received_Status = true;
                    model.Jct_Dak_Register_Recieved.Reply_ReferenceDate = replydate;
                    model.Jct_Dak_Register_Recieved.Reply_ReferenceNo = replyrefer;
                    model.Jct_Dak_Register_Recieved.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    model.Jct_Dak_Register_Recieved.Created_On = System.DateTime.Now;
                    model.Jct_Dak_Register_Recieved.Created_Hostname = Environment.MachineName;

                    var current = db.Jct_Dak_Register_Recieved.Single(x => x.Inward_No == model.Jct_Dak_Register_Recieved.Inward_No);

                    if (current != null)
                    {
                        current.Ip_Address = model.Jct_Dak_Register_Recieved.Ip_Address;
                        current.Received_Status = model.Jct_Dak_Register_Recieved.Received_Status;
                        current.Remarks = model.Jct_Dak_Register_Recieved.Remarks;
                        current.Created_Hostname = model.Jct_Dak_Register_Recieved.Created_Hostname;
                        current.Created_On = model.Jct_Dak_Register_Recieved.Created_On;
                        current.Reply_ReferenceDate = model.Jct_Dak_Register_Recieved.Reply_ReferenceDate;
                        current.Reply_ReferenceNo = model.Jct_Dak_Register_Recieved.Reply_ReferenceNo;
                        db.Entry(current).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return Json("Fail" + ex.ToString() , JsonRequestBehavior.AllowGet);
                    
                }
            }
            //return RedirectToAction("Index", "UserAcceptance");

            return Json("Success" , JsonRequestBehavior.AllowGet);
        }

 

    }



}