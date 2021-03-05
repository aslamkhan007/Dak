using DakManSys.Entity;
using DakManSys.Security;
using DakManSys.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Controllers
{
    public class ReportController : Controller
    {
        DRSEntities context = new DRSEntities();
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult MonthlyReport()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DepartmentReport()
        {
            ParentViewModel model = new ParentViewModel();
            model.DropDownViewModel.DeptList = DeptList();
            return View(model);
        }

        //For Binding of Department dropdownList
        public List<SelectListItem> DeptList()
        {
            var dept = context.Jct_Dak_DeptMaster.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in dept)
            {
                list.Add(new SelectListItem { Text = item.DEPTNAME, Value = item.DEPTCODE });
            }
            return list;
        }
        [CustomAuthorize(Roles = "User")]
        public ActionResult UserMonthlyReport()
        {
            return View();
        }

         [CustomAuthorize(Roles = "Admin")]
        public ActionResult ChequeReport()
        {
            return View();
        }

        [CustomAuthorize(Roles = "User")]
        public ActionResult UserDakRecievedReport()
         {
             return View();
         }
        [CustomAuthorize(Roles="Admin")]
        public ActionResult ParcelReport()
        {
            return View();
        }

    }
}