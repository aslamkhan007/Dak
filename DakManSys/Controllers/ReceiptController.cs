using DakManSys.ADOConnection;
using DakManSys.Entity;
using DakManSys.Security;
using DakManSys.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;


namespace DakManSys.Controllers
{
    public class ReceiptController : Controller
    {
        DRSEntities context = new DRSEntities();
        //
        // GET: /Receipt/
        [CustomAuthorize(Roles = "Admin,User")]
        public ActionResult Index()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult NewDakEntry()
        {
            try
            {
                ParentViewModel model = new ParentViewModel();
                //for DakType Dropdown Binding
                model.DropDownViewModel.DakType = DakType();
                //for MailMode DropDown Binding
                model.DropDownViewModel.MailMode = ServiceMode();
                //for Mail service DropdownList
                model.DropDownViewModel.MailService = MailService();
                //for CountryDropdownList 
                model.DropDownViewModel.CountryList = CountryList();
                //fOR stateDropDownList
                model.DropDownViewModel.StateList = StateList();
                //for cityDropDownList
                model.DropDownViewModel.CityList = cityList();
                //for HodDropDownlist
                model.DropDownViewModel.HodList = HodList();
                //for DeptDropDownList 
                model.DropDownViewModel.DeptList = DeptList();
                if (TempData["PartyHeader"] != null && TempData["PartyAddress"] != null)
                {
                    model.Jct_Dak_Party_Header = (Jct_Dak_Party_Header)TempData["PartyHeader"];
                    model.Jct_Dak_Party_Detail_Address = (Jct_Dak_Party_Detail_Address)TempData["PartyAddress"];
                }
                model.DropDownViewModel.BankList = BankList();
                return View("Welcome", model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }
        }

        //To bind the DakType Dropdownlist       
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
        //To bind the DakType Dropdownlist       
        public List<SelectListItem> DakType()
        {
            var daktype = context.Jct_Dak_Itemtype.Select(x => new { x.Dak_Code, x.DakType }).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in daktype)
            {
                list.Add(new SelectListItem { Value = item.Dak_Code.ToString(), Text = item.DakType });
            }
            return list;
        }
        //To bind  MailMode Dropdownlist
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
        //To bind Mail Service Dropdownlist
        public List<SelectListItem> MailService()
        {
            var mailservice = context.Jct_Dak_Service_Details.Where(y => y.ServiceMode_Code == "CO").Select(x => new { x.DakService_Code, x.DakService_Name });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in mailservice)
            {
                list.Add(new SelectListItem { Value = item.DakService_Code, Text = item.DakService_Name });
            }
            return list;
        }

        //For Conditional binding of Mail service Dropdownlist
        [HttpPost]
        public JsonResult ServiceModeBind(string MailMode)
        {

            var conditionalList = context.Jct_Dak_Service_Details.Where(x => x.ServiceMode_Code == MailMode).Select(y => new { y.DakService_Code, y.DakService_Name });
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in conditionalList)
            {
                list.Add(new SelectListItem { Value = item.DakService_Name, Text = item.DakService_Name });
            }
            return Json(list, JsonRequestBehavior.AllowGet);


        }

        //For binding of country Dropdownlist
        public List<SelectListItem> CountryList()
        {
            var countrylist = context.Jct_Dak_Country.ToList();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in countrylist)
            {
                if (item.name.Equals("India"))
                {
                    list.Add(new SelectListItem { Value = item.name, Text = item.name, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem { Value = item.name, Text = item.name });
                }
            }
            return list;
        }

        // For binding StateDropdownlist in case of India
        public List<SelectListItem> StateList()
        {
            var StateList = context.Jct_Dak_State.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in StateList)
            {
                list.Add(new SelectListItem { Value = item.state, Text = item.state });
            }
            return list;

        }
        [HttpPost]
        public JsonResult ConditionalCityList(string State)
        {
            var city = context.Jct_Dak_Cities.Where(x => x.city_state == State).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in city)
            {
                list.Add(new SelectListItem { Value = item.city_name, Text = item.city_name });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //For Binding of City dropdownList in case of india 
        public List<SelectListItem> cityList()
        {
            var city = context.Jct_Dak_Cities.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in city)
            {
                list.Add(new SelectListItem { Value = item.city_name, Text = item.city_name });
            }
            return list;
        }
        //For Binding of Hod List 
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
        [HttpPost]
        public JsonResult HodDetails(string Hod)
        {
            var detail = context.JCT_EMP_HOD.Where(x => x.Hod_Code == Hod).Select(y => new { y.Mobile, y.E_MailID }).FirstOrDefault();

            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        //For Binding of Department dropdownList
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
        [HttpPost]
        public JsonResult fetchHod(string dept)
        {
            var hod = context.JCT_EMP_HOD.Where(x => x.Deptcode == dept && x.Eff_To > System.DateTime.Now).Select(y => new { y.Hod_Code, y.Hod_Name }).ToList();
            return Json(hod, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult PartyAutocomplete(string term, string flag)
        {
            if (flag.Equals("O", StringComparison.InvariantCultureIgnoreCase))
            {
                var result = new List<KeyValuePair<string, string>>();
                var partyResult = context.Jct_Dak_Party_Header.Where(x => x.Party_Type == flag).Select(y => new { y.Party_Code, y.Party_Name }).ToList();
                foreach (var item in partyResult)
                {
                    result.Add(new KeyValuePair<string, string>(item.Party_Code, item.Party_Name));
                }
                var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            else if (flag.Equals("S", StringComparison.InvariantCultureIgnoreCase))
            {
                var result = new List<KeyValuePair<string, string>>();
                var partyResult = context.Jct_Dak_Party_Header.Where(x => x.Party_Type == flag).Select(y => new { y.Party_Code, y.Party_Name }).ToList();
                foreach (var item in partyResult)
                {
                    result.Add(new KeyValuePair<string, string>(item.Party_Code, item.Party_Name));
                }
                var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            else if (flag.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                var result = new List<KeyValuePair<string, string>>();
                var partyResult = context.Jct_Dak_Party_Header.Where(x => x.Party_Type == flag).Select(y => new { y.Party_Code, y.Party_Name }).ToList();
                foreach (var item in partyResult)
                {
                    result.Add(new KeyValuePair<string, string>(item.Party_Code, item.Party_Name));
                }
                var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new List<KeyValuePair<string, string>>();
                var partyResult = context.Jct_Dak_Party_Header.Select(y => new { y.Party_Code, y.Party_Name }).ToList();
                foreach (var item in partyResult)
                {
                    result.Add(new KeyValuePair<string, string>(item.Party_Code, item.Party_Name));
                }
                var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                return Json(result3, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult PartyDetails(string partycode)
        {
            var partydetails = context.Jct_Dak_Party_Detail_Address.Where(x => x.Party_Code == partycode).ToList();
            return Json(new { Result = "OK", Records = partydetails, TotalRecordCount = partydetails.Count });

        }
        [HttpPost]
        public JsonResult PartyType(string partycode, string partyname)
        {
            var partytype = context.Jct_Dak_Party_Header.Where(x => x.Party_Code == partycode && x.Party_Name == partyname
                ).Select(y => y.Party_Type).Distinct();
            return Json(partytype, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getEmployeeMail(string term, string dept)
        {
            var result = new List<KeyValuePair<string, string>>();
            SqlConnection con = DBConnection.getConnection();
            string sql = "Select empcode,Name from mistel";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader read = cmd.ExecuteReader();
            if (read.HasRows)
            {
                while (read.Read())
                {
                    result.Add(new KeyValuePair<string, string>(read[1].ToString() + "(" + read[0].ToString() + ")", read[1].ToString() + "(" + read[0].ToString() + ")"));
                }
            }


            //var details = context.jct_Dak_Email.Select(y => new { y.EMail_ID, y.Emp_Name }).ToList();
            //foreach (var item in details)
            //{
            //    result.Add(new KeyValuePair<string, string>(item.Emp_Name, item.Emp_Name));
            //}
            var result3 = result.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(result3, JsonRequestBehavior.AllowGet);
        }
        public string getPartyRecord(string name)
        {
            var partycode = context.Jct_Dak_Party_Sr_No.Select(x => x.PartyNo).First();
            string newname = name.Substring(0, 3).ToUpper() + partycode.ToString();
            return newname;
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult AddSender(ParentViewModel parent)
        {
            try
            {
                Jct_Dak_Party_Header model = new Jct_Dak_Party_Header();
                Jct_Dak_Party_Detail_Address model1 = new Jct_Dak_Party_Detail_Address();


                //Party code generation 
                string partycode = getPartyRecord(parent.Jct_Dak_Party_Header.Party_Name);

                //----------------------------////
                model1.address_id = "1";
                model1.Address_Line_1 = parent.Jct_Dak_Party_Detail_Address.Address_Line_1;
                model1.Party_Code = partycode;
                model1.Address_Line_2 = parent.Jct_Dak_Party_Detail_Address.Address_Line_2;
                model1.Address_Line_3 = parent.Jct_Dak_Party_Detail_Address.Address_Line_3;
                model1.Email = parent.Jct_Dak_Party_Detail_Address.Email;
                model1.Fax_No = parent.Jct_Dak_Party_Detail_Address.Fax_No;
                model1.Telex_No = parent.Jct_Dak_Party_Detail_Address.Zip_No;
                model1.Country = parent.DropDownViewModel.Jct_Dak_Country.name;
                model1.State = parent.Jct_Dak_Party_Detail_Address.State;
                if (parent.Jct_Dak_Party_Detail_Address.City == "Select City")
                {
                    model1.City = string.Empty;
                }
                else
                {

                    model1.City = parent.Jct_Dak_Party_Detail_Address.City;
                }
                model1.Party_Type = "O";
                model1.Zip_No = parent.Jct_Dak_Party_Detail_Address.Zip_No;
                model1.Phone_No = parent.Jct_Dak_Party_Detail_Address.Phone_No;
                model1.Empcode = HttpContext.User.Identity.Name.ToString();
                model1.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                model1.Created_On = System.DateTime.Now;
                model1.Created_Hostname = Environment.MachineName;


                //Jct_Dak_PArty_Header model
                model.Party_Name = parent.Jct_Dak_Party_Header.Party_Name;
                model.Party_Type = "O";
                model.Remarks = parent.Jct_Dak_Party_Header.Remarks;
                model.Party_Code = partycode;
                model.Empcode = HttpContext.User.Identity.Name.ToString();
                model.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                model.Created_On = System.DateTime.Now;
                //model.Created_Hostname = Environment.MachineName;
                model.Created_Hostname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");


                context.Jct_Dak_Party_Header.Add(model);
                context.Jct_Dak_Party_Detail_Address.Add(model1);
                context.SaveChanges();
                //for incrementing the party Code Number
                var a = context.Jct_Dak_Party_Sr_No.Select(x => new { x.PartyNo, x.TransNo }).First();
                parent.Jct_Dak_Party_Sr_No.PartyNo = (Convert.ToInt64(a.PartyNo) + 1).ToString().PadLeft(4, '0');
                parent.Jct_Dak_Party_Sr_No.TransNo = a.TransNo;
                using (var db = new DRSEntities())
                {
                    db.Jct_Dak_Party_Sr_No.Attach(parent.Jct_Dak_Party_Sr_No);
                    db.Entry(parent.Jct_Dak_Party_Sr_No).Property(x => x.PartyNo).IsModified = true;
                    db.SaveChanges();
                }

                TempData["PartyHeader"] = model;
                TempData["PartyAddress"] = model1;

                return RedirectToAction("NewDakEntry", "Receipt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("NewDakEntry", "Receipt");
            }

        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DakEntry(ParentViewModel model)
        {
            ParentViewModel newModel = new ParentViewModel();
            string msg = "The record was not added successfully";
            char status = 'F';
            model.EntryDate = System.DateTime.Now;
            try
            {
                //for fetching the DAK serial Number
                var a = context.Jct_Dak_Type_Sr_No.Where(x => x.EffectiveTo >= model.EntryDate).FirstOrDefault();
                newModel.Jct_Dak_Type_Sr_No.Postfix = a.Postfix;
                newModel.Jct_Dak_Type_Sr_No.Prefix = a.Prefix;
                newModel.Jct_Dak_Type_Sr_No.LastNo = a.LastNo;
                newModel.Jct_Dak_Type_Sr_No.Delimiter = a.Delimiter;
                newModel.Sr_No = newModel.Jct_Dak_Type_Sr_No.Prefix + newModel.Jct_Dak_Type_Sr_No.Delimiter + newModel.Jct_Dak_Type_Sr_No.LastNo + newModel.Jct_Dak_Type_Sr_No.Delimiter + newModel.Jct_Dak_Type_Sr_No.Postfix;

                //For saving of the submited form
                Jct_Dak_Register register = new Jct_Dak_Register();
                register.Dak_Code = model.Jct_Dak_Register.Dak_Code;
                register.Dak_Type = model.Jct_Dak_Register.Dak_Type;
                register.Inward_No = newModel.Sr_No;
                register.ReferenceNo = model.Jct_Dak_Register.ReferenceNo;
                register.DakService_Code = model.Jct_Dak_Register.DakService_Code;
                register.DakService_Type = model.Jct_Dak_Register.DakService_Type;
                register.Reference_Date = model.Jct_Dak_Register.Reference_Date;
                register.Dept_Code = model.JCT_EMP_HOD.Deptcode;
                register.Department = model.Jct_Dak_Register.Department;
                register.Hod_Code = model.Jct_Dak_Register.Hod_Code;
                register.SUBJECT = model.Jct_Dak_Register.SUBJECT;
                register.Party_Code = model.Jct_Dak_Party_Header.Party_Code.Trim();
                register.MailTo = model.Jct_Dak_Register.MailTo;
                register.ConcernedPersonals = model.Jct_Dak_Register.ConcernedPersonals!=null?model.Jct_Dak_Register.ConcernedPersonals:"";
                register.Phone_No = model.Jct_Dak_Register.Phone_No;
                register.Remarks = model.Jct_Dak_Register.Remarks;
                if (model.AddressID.Equals("") || model.AddressID.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                {

                }
                register.PartyAddress_id = model.AddressID;
                register.Email_Status = false;
                register.Party_Type = model.Jct_Dak_Register.Party_Type;
                register.Empcode = HttpContext.User.Identity.Name.ToString();
                register.DakService_Mode = model.Jct_Dak_Register.DakService_Mode;
                register.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                register.Created_On = System.DateTime.Today;
                register.Created_Hostname = Environment.MachineName;
                if (model.Jct_Dak_Register.Dak_Code.Equals("CHQ", StringComparison.InvariantCultureIgnoreCase))
                {
                    Jct_Dak_Register_Cheque cheque = new Jct_Dak_Register_Cheque();
                    cheque.Inward_No = newModel.Sr_No;
                    cheque.Cheque_No = model.Jct_Dak_Register_Cheque.Cheque_No;
                    cheque.Cheque_Date = model.Jct_Dak_Register_Cheque.Cheque_Date;
                    cheque.BankName = model.Jct_Dak_Register_Cheque.BankName;
                    cheque.Amount = model.Jct_Dak_Register_Cheque.Amount;
                    cheque.Payable_At = model.Jct_Dak_Register_Cheque.Payable_At;
                    cheque.Empcode = HttpContext.User.Identity.Name.ToString();
                    cheque.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    cheque.Created_On = System.DateTime.Today;
                    cheque.Created_Hostname = Environment.MachineName;
                    context.Jct_Dak_Register_Cheque.Add(cheque);
                }

                context.Jct_Dak_Register.Add(register);
                Jct_Dak_Register_Recieved dakrecieved = new Jct_Dak_Register_Recieved();
                dakrecieved.Inward_No = newModel.Sr_No;
                dakrecieved.Received_Status = false;
                dakrecieved.Email_Status = false;
                context.Jct_Dak_Register_Recieved.Add(dakrecieved);
                context.SaveChanges();

                var ab = context.Jct_Dak_Type_Sr_No.Where(x => x.EffectiveTo >= model.EntryDate).Select(x => new { x.LastNo, x.TransNo }).First();
                model.Jct_Dak_Type_Sr_No.LastNo = (Convert.ToInt64(ab.LastNo) + 1).ToString().PadLeft(6, '0');
                model.Jct_Dak_Type_Sr_No.TransNo = ab.TransNo;
                using (var db = new DRSEntities())
                {
                    db.Jct_Dak_Type_Sr_No.Attach(model.Jct_Dak_Type_Sr_No);
                    db.Entry(model.Jct_Dak_Type_Sr_No).Property(x => x.LastNo).IsModified = true;
                    db.SaveChanges();
                }


                if (register.ConcernedPersonals!=string.Empty || register.ConcernedPersonals.Equals("null",StringComparison.InvariantCultureIgnoreCase))
                {
                    string empcode = register.ConcernedPersonals.Split('(', ')')[1];
                    SqlConnection con;


                    con = DBConnection.getConnection();

                    string sql = "select E_MailID  from mistel where empcode='" + empcode + "'";
                    string email = string.Empty;
                    SqlCommand cmd = new SqlCommand(sql, con);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        email = read.GetString(0);
                    }
                    read.Close();
                    con.Close();
                    if (email != string.Empty)
                    {
                        string to = email;
                        string from = "noreply@jctltd.com";
                        string bcc = "aslam@jctltd.com";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<html>");
                        sb.AppendLine("<head>");
                        sb.AppendLine("</head>");
                        sb.AppendLine("<body>");
                        sb.AppendLine("<h3>A Dak addressed to you has been recieved by the Admin Department.The Detail of the given Dak is as follows:</h3><br/>");
                        sb.AppendLine("<table>");
                        sb.AppendLine("<tr>");
                        sb.AppendLine("<td>");
                        sb.AppendLine("<strong> Dak Inward Number : </strong> ");
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(newModel.Sr_No);
                        sb.AppendLine("</td>");
                        sb.AppendLine("</tr>");
                        sb.AppendLine("<tr>");
                        sb.AppendLine("<td>");
                        sb.AppendLine("<strong>DakType :</strong>");
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(model.Jct_Dak_Register.Dak_Type);
                        sb.AppendLine("</td>");

                        sb.AppendLine("</tr>");


                        sb.AppendLine("<tr>");
                        sb.AppendLine("<td>");
                        sb.AppendLine("<strong>PartyName :</strong>");
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(model.Jct_Dak_Party_Header.Party_Name);
                        sb.AppendLine("</td>");

                        sb.AppendLine("</tr>");
                       
                        sb.AppendLine("</table><br/><br/><br/><br/>");
                        //sb.AppendLine("<p>Please Contact to ");
                        sb.AppendLine("<p>Note:-Please do not reply to this mail.This is a system generated mail.</p>");


                        sb.AppendLine("</body>");
                        sb.AppendLine("</html>");
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(from);

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
                        mail.To.Add(new MailAddress(to));
                        mail.Subject = register.SUBJECT;
                        mail.Body = sb.ToString();
                        mail.IsBodyHtml = true;
                        SmtpClient SmtpMail = new SmtpClient("exchange2k7");
                        SmtpMail.Send(mail);

                    }
                }
                //Dak Mail Functionality
                //model.Jct_Dak_Register.Inward_No = newModel.Sr_No;
                

                //-------------------------------///
                msg = "The record was added successfully with Dak Number    " + newModel.Sr_No + "";
                //var result = new { message = msg, inwardno = model.Sr_No };
                return Json(msg, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                msg = msg + "Exception:" + e;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

        }


        //------------Method for adding new party Address against a partycode-------//
        public JsonResult NewPartyAddress(string NewAddress, string partyCode, string PartyType)
        {
            string msg="New Address Not Added";
            
            using (var db = new DRSEntities())
            {
                try{
                    dynamic ab = JsonConvert.DeserializeObject(NewAddress);

                    int addressid = Convert.ToInt32(db.Jct_Dak_Party_Detail_Address.Where(y => y.Party_Code == partyCode).Max(x => x.address_id)) + 1;
                    Jct_Dak_Party_Detail_Address model = new Jct_Dak_Party_Detail_Address();
                    model.Party_Code = partyCode;
                    model.Party_Type = PartyType;
                    model.address_id = addressid.ToString();
                    model.Address_Line_1 = ab.txtaddress1;
                    model.Address_Line_2 = ab.txtaddress2;
                    model.Address_Line_3 = ab.txtaddress3;
                    model.City = ab.txtcity;
                    model.Country = ab.txtcountry;
                    model.Fax_No = ab.txtfax;
                    model.Phone_No = ab.txtphone;
                    model.State = ab.txtstate;
                    model.Telex_No = ab.txttelex;
                    model.Zip_No = ab.txtzip;
                    model.Email = ab.txtemail;
                    model.Empcode = HttpContext.User.Identity.Name.ToString();
                    model.Ip_Address = ("::1" == System.Web.HttpContext.Current.Request.UserHostAddress) ? "localhost" : System.Web.HttpContext.Current.Request.UserHostAddress;
                    model.Created_On = System.DateTime.Today;
                    model.Created_Hostname = Environment.MachineName;

                    db.Jct_Dak_Party_Detail_Address.Add(model);
                    db.SaveChanges();
                    msg = "New Address Added";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                catch(Exception e)
                {
                    return Json(msg + "-Exception:" + e);
                }
                
            }
            
        }
        //---------------------------------------------------------------------------------------------//


    }
}