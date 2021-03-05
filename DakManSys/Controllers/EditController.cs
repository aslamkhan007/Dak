using DakManSys.Entity;
using DakManSys.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DakManSys.Controllers
{
    public class EditController : Controller
    {
        DRSEntities context = new DRSEntities();
        //
        // GET: /Edit/
       
        public ActionResult EditDetails(string DakNo)
        {
            ParentViewModel model=new ParentViewModel();

            model.DropDownViewModel.BankList=BankList();
            model.DropDownViewModel.DeptList = DeptList();
            model.DropDownViewModel.HodList = HodList();
            model.DropDownViewModel.MailMode = ServiceMode();
            model.DropDownViewModel.MailService = MailService();
              using (var db = new DRSEntities())
                {

                    var query = (from m in db.Jct_Dak_Register
                                 join a in db.JCT_EMP_HOD on m.Hod_Code equals a.Hod_Code
                                 join b in db.Jct_Dak_Party_Header on m.Party_Code equals b.Party_Code
                                 join d in db.Jct_Dak_Party_Detail_Address on m.Party_Code equals d.Party_Code
                                 join c in db.Jct_Dak_Register_Cheque on m.Inward_No equals c.Inward_No into recieve
                                 where m.Inward_No == DakNo
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
              }
            return View(model);
        }

        public List<SelectListItem> ServiceMode()
        {
            var dakmode = context.Jct_Dak_Service_Mode.Where(x=>x.Status=="Y").Select(x => new { x.ServiceMode_Code, x.ServiceMode_Name }).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in dakmode)
            {
                list.Add(new SelectListItem { Value = item.ServiceMode_Code, Text = item.ServiceMode_Name });
            }
            return list;
        }
        public List<SelectListItem> DeptList()
        {
            var dept = context.Jct_Dak_DeptMaster.Where(x=>x.Status=="Y").ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in dept)
            {
                list.Add(new SelectListItem { Text = item.DEPTNAME, Value = item.DEPTCODE });
            }
            return list;
        }
        public List<SelectListItem> HodList()
        {
            var hod = context.JCT_EMP_HOD.Select(x => new { x.Hod_Name, x.Hod_Code });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in hod)
            {
                list.Add(new SelectListItem { Value = item.Hod_Code, Text = item.Hod_Name });
            }
            return list;
        }
                public List<SelectListItem> MailService()
        {
            var mailservice = context.Jct_Dak_Service_Details.Select(x => new { x.DakService_Code, x.DakService_Name });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in mailservice)
            {
                list.Add(new SelectListItem { Value = item.DakService_Name, Text = item.DakService_Name });
            }
            return list;
        }
        public List<SelectListItem> BankList()
        {
            var bankname = context.Jct_Dak_BankMaster.Where(y=>y.Status=="Y").Select(x => new { x.BankName, x.BankCode }).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in bankname)
            {
                list.Add(new SelectListItem { Value = item.BankName.ToString(), Text = item.BankName });
            }
            return list;
        }
        [HttpPost]
        public JsonResult fetchHod(string dept)
        {
            var hod = context.JCT_EMP_HOD.Where(x => x.Deptcode == dept && x.Eff_To > System.DateTime.Now).Select(y => new { y.Hod_Code, y.Hod_Name }).ToList();
            return Json(hod, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult HodDetails(string Hod)
        {
            var detail = context.JCT_EMP_HOD.Where(x => x.Hod_Code == Hod).Select(y => new { y.Mobile, y.E_MailID }).First();

            return Json(detail, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult getEmployeeMail(string term)
        {
            var result = new List<KeyValuePair<string, string>>();
            var details = context.jct_Dak_Email.Select(y => new { y.EMail_ID, y.Emp_Name }).ToList();
            foreach (var item in details)
            {
                result.Add(new KeyValuePair<string, string>(item.Emp_Name, item.Emp_Name));
            }
            var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(result3, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateDak(ParentViewModel model, FormCollection form)
        {
            ParentViewModel obj = new ParentViewModel();
            using (var db = new DRSEntities())
            {

                var query = (from m in db.Jct_Dak_Register
                             join a in db.JCT_EMP_HOD on m.Hod_Code equals a.Hod_Code
                             join b in db.Jct_Dak_Party_Header on m.Party_Code equals b.Party_Code
                             join d in db.Jct_Dak_Party_Detail_Address on m.Party_Code equals d.Party_Code
                             join c in db.Jct_Dak_Register_Cheque on m.Inward_No equals c.Inward_No into recieve
                             where m.Inward_No == model.Jct_Dak_Register.Inward_No
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
                    obj.Jct_Dak_Register = item.Jct_Dak_Register;
                    obj.Jct_Dak_Register_Cheque = item.Jct_Dak_Register_Cheque;
                    obj.Jct_Dak_Party_Header = item.Jct_Dak_Party_Header;
                    obj.Jct_Dak_Party_Detail_Address = item.Jct_Dak_Party_Detail_Address;
                    obj.JCT_EMP_HOD = item.JCT_EMP_HOD;
                }

                obj.Jct_Dak_Register.Dak_Code = obj.Jct_Dak_Register.Dak_Code;
                obj.Jct_Dak_Register.Dak_Type = obj.Jct_Dak_Register.Dak_Type;
                obj.Jct_Dak_Register.Inward_No = obj.Jct_Dak_Register.Inward_No;
                obj.Jct_Dak_Register.ReferenceNo = model.Jct_Dak_Register.ReferenceNo;
                obj.Jct_Dak_Register.DakService_Code = model.Jct_Dak_Register.DakService_Code;
                obj.Jct_Dak_Register.DakService_Type = model.Jct_Dak_Register.DakService_Type;
                obj.Jct_Dak_Register.Reference_Date = model.Jct_Dak_Register.Reference_Date;
                obj.Jct_Dak_Register.Dept_Code = model.JCT_EMP_HOD.Deptcode;
                obj.Jct_Dak_Register.Department = model.Jct_Dak_Register.Department;
                //register.Hod_Code = model.Jct_Dak_Register.Hod_Code;
                obj.Jct_Dak_Register.Hod_Code = form["JCT_EMP_HOD.Hod_Code"].ToString().Replace(',', ' ').Trim();
                obj.Jct_Dak_Register.SUBJECT = model.Jct_Dak_Register.SUBJECT;
                obj.Jct_Dak_Register.Party_Code = model.Jct_Dak_Register.Party_Code.Trim();
                obj.Jct_Dak_Register.MailTo = model.Jct_Dak_Register.MailTo;
                obj.Jct_Dak_Register.ConcernedPersonals = model.Jct_Dak_Register.ConcernedPersonals;
                obj.Jct_Dak_Register.Phone_No = model.Jct_Dak_Register.Phone_No;
                obj.Jct_Dak_Register.Remarks = model.Jct_Dak_Register.Remarks;
                obj.Jct_Dak_Register.PartyAddress_id = obj.Jct_Dak_Register.PartyAddress_id;
                obj.Jct_Dak_Register.Email_Status = false;
                obj.Jct_Dak_Register.Party_Type = obj.Jct_Dak_Register.Party_Type;
                obj.Jct_Dak_Register.Empcode = HttpContext.User.Identity.Name;
                obj.Jct_Dak_Register.DakService_Mode = model.Jct_Dak_Register.DakService_Mode;
                obj.Jct_Dak_Register.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                obj.Jct_Dak_Register.Created_On = System.DateTime.Today;
                obj.Jct_Dak_Register.Created_Hostname = Environment.MachineName;
                if (model.Jct_Dak_Register.Dak_Code.Equals("CHQ", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj.Jct_Dak_Register_Cheque.Inward_No = obj.Jct_Dak_Register_Cheque.Inward_No;
                    obj.Jct_Dak_Register_Cheque.Cheque_No = model.Jct_Dak_Register_Cheque.Cheque_No;
                    obj.Jct_Dak_Register_Cheque.Cheque_Date = model.Jct_Dak_Register_Cheque.Cheque_Date;
                    obj.Jct_Dak_Register_Cheque.BankName = model.Jct_Dak_Register_Cheque.BankName;
                    obj.Jct_Dak_Register_Cheque.Amount = model.Jct_Dak_Register_Cheque.Amount;
                    obj.Jct_Dak_Register_Cheque.Payable_At = model.Jct_Dak_Register_Cheque.Payable_At;
                    obj.Jct_Dak_Register_Cheque.Empcode = HttpContext.User.Identity.Name;
                    obj.Jct_Dak_Register_Cheque.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    obj.Jct_Dak_Register_Cheque.Created_On = System.DateTime.Today;
                    obj.Jct_Dak_Register_Cheque.Created_Hostname = Environment.MachineName;
                    db.Entry(obj.Jct_Dak_Register_Cheque).State = EntityState.Modified;
                }

                context.Entry(obj.Jct_Dak_Register).State = EntityState.Modified;
                context.SaveChanges();



            }
            ParentViewModel newModel = new ParentViewModel();
            string msg = "The record was not added successfully";
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("Detail", "UserAcceptance", new { inwardNo = model.Jct_Dak_Register.Inward_No,flag="edit" });
        }
        }

	}