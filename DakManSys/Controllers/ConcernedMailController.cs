using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Controllers
{
    public class ConcernedMailController : Controller
    {
        //
        // GET: /ConcernedMail/
        public ActionResult ConcernedMail()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendMail()
        {
            Thread.Sleep(3000);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
	}
}