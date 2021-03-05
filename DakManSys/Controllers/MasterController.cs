using DakManSys.ADOConnection;
using DakManSys.Entity;
using DakManSys.Models.MasterModel;
using DakManSys.Security;
using DakManSys.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Controllers
{
    public class MasterController : Controller
    {
 
        //
        // GET: /Master/
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult BankMaster()
        {
            return View();
        }
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DakServiceMaster()
        {
                ParentViewModel model = new ParentViewModel();
                model.DropDownViewModel.DakServiceModeList = getDakServiceModeList();
                return View(model);
            
        }

        public List<SelectListItem> getDakServiceModeList()
        {
            using(var db=new DRSEntities())
            {
                List<SelectListItem> list = new List<SelectListItem>();
                var data = db.Jct_Dak_Service_Mode.Where(x => x.Status == "Y").Select(y=>new {y.ServiceMode_Code,y.ServiceMode_Name});
                foreach(var item in data)
                {
                    list.Add(new SelectListItem { Text = item.ServiceMode_Name, Value = item.ServiceMode_Code });
                }
                return list;
            }
        }
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DepartmentMaster()
        {
            return View();
        }
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult EmployeeMappingMaster()
        {
            EmployeeMappingModel model = new EmployeeMappingModel();
            using(var db=new DRSEntities())
            {
                var deptlist = db.Jct_Dak_DeptMaster.Select(x => new { x.DEPTCODE, x.DEPTNAME }).ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach(var item in deptlist)
                {
                    list.Add(new SelectListItem { Text = item.DEPTNAME, Value = item.DEPTCODE.ToString() });
                }
                model.Deptlist = list;
            }
           
            return View(model);
        }

        public ActionResult DakServiceModeMaster ()
        {
            return View();
        }



        //POST:/MASTER/----------------------------------------------------------------
        [HttpPost]
        public ActionResult DakServiceModeMaster(DakServiceModeMaster model)
        {
            try
            {
                using (var db = new DRSEntities())
                {

                    Jct_Dak_Service_Mode obj = new Jct_Dak_Service_Mode();
                    var id = Convert.ToInt32(db.Jct_Dak_Service_Mode.Max(x => x.TransNo)) + 1;
                    obj.ServiceMode_Name = model.DakServiceModeName;
                    obj.ServiceMode_Code = model.DakServiceModeName.Substring(0, 3) + id;
                    obj.Status = "Y";
                    obj.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    obj.Created_Hostname = Environment.MachineName;
                    obj.Created_On = System.DateTime.Now;
                    obj.Empcode = HttpContext.User.Identity.Name;
                    db.Jct_Dak_Service_Mode.Add(obj);
                    db.SaveChanges();

                }
                return RedirectToAction("DakServiceModeMaster", "Master", null);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Unable to Add the Information");
                Console.Write(e.ToString());
                return View("DakServiceModeMaster");
            }
        } 

        [HttpPost]
        public ActionResult DakServiceMaster(ParentViewModel obj)
        {
            try
            {
                using (var db = new DRSEntities())
                {
                    var servicecode = Convert.ToInt32(db.Jct_Dak_Service_Details.Max(x => x.DakService_Code)) + 1;
                    Jct_Dak_Service_Details model = new Jct_Dak_Service_Details();
                    model.Address_Line_1 = obj.DakServiceModel.Address_Line_1;
                    model.Address_Line_2 = obj.DakServiceModel.Address_Line_2;
                    model.Address_Line_3 = obj.DakServiceModel.Address_line_3;
                    model.City = obj.DakServiceModel.city;
                    model.Country = obj.DakServiceModel.Country;
                    model.State = obj.DakServiceModel.State;
                    model.DakService_Code = servicecode.ToString();
                    model.DakService_Name = obj.DakServiceModel.DakService_Name;
                    model.Description = obj.DakServiceModel.Description;
                    model.Email_Address = obj.DakServiceModel.Email_Address;
                    model.FaxNumber = obj.DakServiceModel.Fax_Number;
                    model.Office_Number = obj.DakServiceModel.Office_Number;
                    model.PhoneNo = obj.DakServiceModel.PhoneNo;
                    model.ServiceMode_Code = obj.DakServiceModel.ServiceMode_Code;
                    model.WebSite = obj.DakServiceModel.Website;
                    model.ZipCode = obj.DakServiceModel.ZipCode;
                    model.Empcode = HttpContext.User.Identity.Name;
                    model.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    model.Created_On = System.DateTime.Now;
                    model.Created_Hostname = Environment.MachineName;
                    model.EffectiveFrom = System.DateTime.Now;
                    model.EffectiveTo = System.DateTime.MaxValue;
                    db.Jct_Dak_Service_Details.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("DakServiceMaster", "Master", null);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("","Unable to Add the Information");
                Console.Write(e.ToString());
            }
            return View("DakServiceMaster");
        }


        [HttpPost]
        public  ActionResult BankMaster(BankMasterModel obj)
        {
            try
            {
                Jct_Dak_BankMaster model = new Jct_Dak_BankMaster();
                using (var db = new DRSEntities())
                {
                    var id=Convert.ToInt32(db.Jct_Dak_BankMaster.Max(x=>x.TransNo).ToString())+1;
                    model.BankName = obj.BankName;
                    model.BankCode = obj.BankName.Substring(0, 3)+id.ToString();
                    model.Empcode = HttpContext.User.Identity.Name.ToString();
                    model.Dated = System.DateTime.Now;
                    model.Status = "Y";
                    db.Jct_Dak_BankMaster.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("BankMaster", "Master", null);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Unable to Add the Information");
                Console.Write(e.ToString());
            }
            return View("BankMaster");
        }

        [HttpPost]
        public ActionResult DepartmentMaster(DepartmentMasterModel obj)
        {
            try
            {
                Jct_Dak_DeptMaster model = new Jct_Dak_DeptMaster();
                using (var db = new DRSEntities())
                {
                    model.DEPTNAME = obj.Department;
                    model.DEPTCODE = obj.Department.Substring(0, 3);
                    model.Status = "Y";
                    db.Jct_Dak_DeptMaster.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("DepartmentMaster", "Master", null);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Unable to Add the Information");
                Console.Write(e.ToString());
            }
            return View("DepartmentMaster");
        }


        [HttpPost]
        public ActionResult EmployeeMappingMaster(EmployeeMappingModel obj)
        {
            EmployeeMappingModel model1 = new EmployeeMappingModel();

            try
            {
                
                
                JCT_EMP_HOD model = new JCT_EMP_HOD();
                using(var db=new DRSEntities())
                {
                    var deptlist = db.Jct_Dak_DeptMaster.Select(x => new { x.DEPTCODE, x.DEPTNAME }).ToList();
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (var item in deptlist)
                    {
                        list.Add(new SelectListItem { Text = item.DEPTNAME, Value = item.DEPTCODE.ToString() });
                    }
                    model1.Deptlist = list;

                    var count = db.JCT_EMP_HOD.Where(m => m.Hod_Code == obj.EmployeeCode && m.Status=="Y").Count();

                    if (count == 0)
                    {

                        model.Deptcode = obj.Department;
                        model.Hod_Code = obj.EmployeeCode;
                        model.Hod_Name = obj.EmployeeName;
                        model.Mobile = obj.MobileNo;
                        model.E_MailID = obj.Email;
                        model.Flag = "A";
                        model.Status = "Y";
                        model.Eff_From = DateTime.Today;
                        model.Eff_To = DateTime.MaxValue;
                        db.JCT_EMP_HOD.Add(model);
                        db.SaveChanges();
                        return RedirectToAction("EmployeeMappingMaster", "Master", null);

                    }
                    else
                    {
                        ModelState.AddModelError("", "User Already Mapped ");
                    }
                                   }
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Unable to Add the Information");
                Console.Write(e.ToString());
            }


            return View("EmployeeMappingMaster",model1);
        }

        //-----------------Delete Bank Master------------//
        public JsonResult BankDetails(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            using (var db = new DRSEntities())
            {
                IEnumerable<Jct_Dak_BankMaster> List = (from m in db.Jct_Dak_BankMaster
                                                        where m.Status == "Y"
                                                        select m
                                                        ).AsQueryable();
                int temp = List.Count();
                if (string.IsNullOrEmpty(jtSorting) || jtSorting.Equals("BankName ASC"))
                {
                    List = List.OrderBy(p => p.BankName);
                }
                List = List.Skip(jtStartIndex).Take(jtPageSize);

                List = List.ToList();

                return Json(new { Result = "OK", Records = List, TotalRecordCount = temp });
            }
        }
        public JsonResult BankDelete(string BankCode)
        {
            using (var db = new DRSEntities())
            {
                Jct_Dak_BankMaster deleteBank = db.Jct_Dak_BankMaster.First(x => x.BankCode == BankCode);
                deleteBank.Status = "N";
                db.SaveChanges();


                var List = db.Jct_Dak_BankMaster.Where(x => x.Status == "Y").ToList();
                return Json(new { Result = "OK", Records = List, TotalRecordCount = List.Count });
            }
        }

        //-----------------------------------------------//
        //------------------------ Delete Department Master------------//
        public JsonResult DepartmentDetails(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            using (var db = new DRSEntities())
            {
                IEnumerable<Jct_Dak_DeptMaster> List = (from m in db.Jct_Dak_DeptMaster
                                                        where m.Status == "Y"
                                                        select m
                                                        ).AsQueryable();
                int temp = List.Count();
                if (string.IsNullOrEmpty(jtSorting) || jtSorting.Equals("DEPTNAME ASC"))
                {
                    List = List.OrderBy(p => p.DEPTNAME);
                }
                List = List.Skip(jtStartIndex).Take(jtPageSize);

                List = List.ToList();

                return Json(new { Result = "OK", Records = List, TotalRecordCount = temp });
            }
        }

        public JsonResult DepartmentDelete(string DEPTCODE)
        {
            using (var db = new DRSEntities())
            {
                Jct_Dak_DeptMaster deleteDepartment = db.Jct_Dak_DeptMaster.First(x => x.DEPTCODE == DEPTCODE);
                deleteDepartment.Status = "N";
                db.SaveChanges();


                var List = db.Jct_Dak_DeptMaster.Where(x => x.Status == "Y").ToList();
                return Json(new { Result = "OK", Records = List, TotalRecordCount = List.Count });
            }
        }
        //-----------------------------------------------------//

        //---------------------Dak Service Mode Delete ----------------------------//


        public JsonResult DakServiceModeDetails(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            using (var db = new DRSEntities())
            {
                IEnumerable<Jct_Dak_Service_Mode> List = (from m in db.Jct_Dak_Service_Mode
                                                        where m.Status == "Y"
                                                        select m
                                                        ).AsQueryable();
                int temp = List.Count();
                if (string.IsNullOrEmpty(jtSorting) || jtSorting.Equals("DEPTNAME ASC"))
                {
                    List = List.OrderBy(p => p.ServiceMode_Name);
                }
                List = List.Skip(jtStartIndex).Take(jtPageSize);

                List = List.ToList();

                return Json(new { Result = "OK", Records = List, TotalRecordCount = temp });
            }
        }
        public JsonResult DakServiceModeDelete(string ServiceMode_Code )
        {
            using(var db=new DRSEntities())
            {
                Jct_Dak_Service_Mode deleteDakService = db.Jct_Dak_Service_Mode.First(x => x.ServiceMode_Code == ServiceMode_Code);
                deleteDakService.Status = "N";
                db.SaveChanges();
                var List = db.Jct_Dak_Service_Mode.Where(x => x.Status == "Y").ToList();
                return Json(new { Result = "OK", Records = List, TotalRecordCount = List.Count });
            }
        }
        //----------------------------------------------------------------------------//
        public JsonResult EmployeeDelete(string Hod_Code)
        {
            using (var db = new DRSEntities())
            {
                JCT_EMP_HOD deleteBank = db.JCT_EMP_HOD.FirstOrDefault(x => x.Hod_Code == Hod_Code && x.Status=="Y");
                deleteBank.Status = "N";
                deleteBank.Eff_To = System.DateTime.Today;
                db.SaveChanges();


                var List = db.JCT_EMP_HOD.Where(x => x.Status == "Y").ToList();
                return Json(new { Result = "OK", Records = List, TotalRecordCount = List.Count });
            }
        }

        public JsonResult EmployeeListForDept(string deptCode, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            using(var db=new DRSEntities())
            {
               
                List<JCT_EMP_HOD> List = (from m in db.JCT_EMP_HOD
                                          where m.Deptcode == deptCode && m.Status == "Y"
                                          select m).ToList();
                int temp = List.Count;
                return Json(new { Result = "OK", Records = List, TotalRecordCount = temp });
            }
        }



        public JsonResult GetEmployeeEmail(string EmployeeCode)
        {
            SqlConnection con;
            con = DBConnection.getConnection();
            string result = string.Empty;
            string sql = "Select E_MailID from mistel where  empcode='" + EmployeeCode + "' ";
            SqlCommand cmd = new SqlCommand(sql, con);            
            SqlDataReader read = cmd.ExecuteReader();
            while (read.Read())
            {
                result = read.GetString(0);
            }
            read.Close();
           
            return Json(result, JsonRequestBehavior.AllowGet);

        }

	}
}