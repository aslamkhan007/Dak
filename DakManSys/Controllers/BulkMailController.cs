using DakManSys.ADOConnection;
using DakManSys.Entity;
using DakManSys.Models;
using DakManSys.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DakManSys.Controllers
{
    public class BulkMailController : Controller
    {
        //
        // GET: /BulkMail/
        [CustomAuthorize(Roles="Admin")]
        public ActionResult Index()
        {
            return View();
        }


         [CustomAuthorize(Roles = "Admin")]
        public JsonResult SendMail()
        {
            try { 

            using (var db = new DRSEntities())
            {        
               //Working for MailTo method only
                db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                var qry = (from q in db.Jct_Dak_Register
                           join a in
                               (from k in db.Jct_Dak_Register
                                where k.Email_Status==false
                                group k by k.Dept_Code

                                ) on q.Dept_Code equals a.Key
                           where q.Email_Status == false
                           select new BulkMailModel()
                           {
                               deptCode=q.Dept_Code,
                               //Hod_Code = q.Hod_Code,
                               ////InwardNo=q.Inward_No,
                               //MailTo = q.MailTo,
                               Count = a.Count(),
                           }
                            ).Distinct().ToList();
                //-----------------------------------------------------//



                foreach (var item in qry)
                {
                    string to=string.Empty;
     //-------------Mail Send Functionality-------------//
            
                        if (item.MailCC == null || item.MailCC == "")
                        {
                            MailMessage mail = new MailMessage();
                            SqlConnection con = DBConnection.getConnection();
                            string sql = "SELECT  jct.E_MailID ,rol.empcode FROM jctdev.dbo.MISTEL AS  jct INNER JOIN DRS.dbo.jct_dak_role rol ON rol.empcode=jct.empcode WHERE rol.Dept=@1";
                            SqlCommand cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@1", item.deptCode);
                            SqlDataReader read = cmd.ExecuteReader();
                            if (read.HasRows)
                            {
                                while (read.Read())
                                {
                                    if ( Convert.ToString(read["E_MailID"]).Length >2)
                                    {

                                       
                                            mail.To.Add(new MailAddress(read["E_MailID"].ToString()));
                                       
                                    }
                                   
                                }
                            }
                       //aslam
                            string from = "noreply@jctltd.com";
                            string bcc = "aslam@jctltd.com";
                            string subject = "Dak  Recieved  Information";


                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("<html>");
                            sb.AppendLine("<head>");
                            sb.AppendLine("</head>");
                            sb.AppendLine("<body>");
                            sb.AppendLine("<h3> Daks addressed to your Department has been recieved by the Admin Department.</h3><br/>Kindly accept the recieved dak from the system.If The daks has been already accepted through the system, please ignore this mail.");
                            sb.AppendLine("<table>");
                            sb.AppendLine("<tr>");
                            sb.AppendLine("<td>");
                            sb.AppendLine("<strong> Total Dak Number : </strong> ");
                            sb.AppendLine("</td>");
                            sb.AppendLine("<td>");
                            sb.AppendLine(item.Count.ToString());
                            sb.AppendLine("</td>");

                            sb.AppendLine("</tr>");

                            //sb.AppendLine("<tr>");
                            //sb.AppendLine("<td>");
                            //sb.AppendLine("<strong>DakType :</strong>");
                            //sb.AppendLine("</td>");
                            //sb.AppendLine("<td>");
                            ////sb.AppendLine(model.Jct_Dak_Register.Dak_Type);
                            //sb.AppendLine("</td>");

                            //sb.AppendLine("</tr>");


                            //sb.AppendLine("<tr>");
                            //sb.AppendLine("<td>");
                            //sb.AppendLine("<strong>PartyName :</strong>");
                            //sb.AppendLine("</td>");
                            //sb.AppendLine("<td>");
                            ////sb.AppendLine(model.Jct_Dak_Party_Header.Party_Name);
                            //sb.AppendLine("</td>");

                            //sb.AppendLine("</tr>");
                            sb.AppendLine("</table><br/><br/><br/><br/>");
                            sb.AppendLine("<p>Note:-Please do not reply to this mail.This is a system generated mail.</p>");


                            sb.AppendLine("</body>");
                            sb.AppendLine("</html>");



                            //if (!string.IsNullOrEmpty(cc))
                            //{
                            //    if (cc.Contains(","))
                            //    {
                            //        string[] ccs = cc.Split(',');
                            //        for (int i = 0; i <= ccs.Length - 1; i++)
                            //        {
                            //            mail.CC.Add(new MailAddress(ccs[i]));
                            //        }
                            //    }
                            //    else
                            //    {
                            //        mail.CC.Add(new MailAddress(cc));
                            //    }
                            //}

                            if (!string.IsNullOrEmpty(bcc))
                            {
                                if (bcc.Contains(","))
                                {
                                    string[] bccs = bcc.Split(',');
                                    for (int i = 0; i <= bccs.Length - 1; i++)
                                    {
                                        mail.Bcc.Add(new MailAddress(bccs[i]));
                                    }
                                }
                                else
                                {
                                    mail.Bcc.Add(new MailAddress(bcc));
                                }
                            }

                            mail.From = new MailAddress(from);
                            mail.Subject = subject;
                            mail.Body = sb.ToString();
                            mail.IsBodyHtml = true;
                            SmtpClient SmtpMail = new SmtpClient("exchange2k7");
                            SmtpMail.Send(mail);



                            //----------Update mail Sent Status-------------------//
                            var list = db.Jct_Dak_Register.Where(x => x.Dept_Code == item.deptCode).Select(y => new { y.Email_Status, y.TransNo }).ToList();
                            foreach (var obj in list)
                            {
                                Jct_Dak_Register register = new Jct_Dak_Register();
                                register.TransNo = obj.TransNo;
                                register.Email_Status = true;
                                db.Jct_Dak_Register.Attach(register);
                                db.Entry(register).Property(x => x.Email_Status).IsModified = true;
                                db.SaveChanges();
                            }


                            //----------------------------------------------------//

                        }
                        //-------------------------------------------------//
                    }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
                }
             catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                string msg = "Fail" + e.ToString();
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}