using DakManSys.ADOConnection;
using DakManSys.Entity;
using DakManSys.Models;
using DakManSys.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DakManSys.Controllers
{
    public class AuthenticateController : Controller
    {
        DRSEntities context = new DRSEntities();
        SqlConnection con;
        //
        // GET: /Authenticate/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthenticateModel authen)
        {
            string username = null;
            try
            {
                con = DBConnection.getConnection();

               // string sql = "select empcode  from jct_login_emp where empcode=' + authen.Username + "' and active = 'Y' and ((convert(varchar(8),dateofbirth,112)='" & authen.Password & " ') and  new_pass is null)  or (new_pass is not null and new_pass=convert(varchar(30),convert(varbinary,'" & authen.Password & "')))) ";
               string sql = "select empcode  from jct_login_emp where empcode='" + authen.Username + "' and active = 'Y' and ((convert(varchar(8),dateofbirth,112)='" + authen.Password + " ' and  new_pass is null)  or (new_pass is not null and new_pass=convert(varchar(30),convert(varbinary,'" + authen.Password + "')))) ";

                SqlCommand cmd = new SqlCommand(sql, con);
                //cmd.Parameters.AddWithValue("@name", authen.Username);
                //cmd.Parameters.AddWithValue("@pass", authen.Password);
                SqlDataReader read = cmd.ExecuteReader();
                while(read.Read())
                {
                    username = read.GetString(0);
                }
                read.Close();
                if(!username.Equals(null))
                {
                    var roles = context.jct_dak_role.Where(x => x.empcode == username).Select(m => m.Rolename).ToArray();
                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                    serializeModel.UserId = username;
                    serializeModel.dept = string.Empty;
                    serializeModel.LastName = string.Empty;
                    serializeModel.roles = roles;
                    string userData = JsonConvert.SerializeObject(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                         1,
                        username,
                         DateTime.Now,
                         DateTime.Now.AddMinutes(15),
                         false,
                         userData);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);
                    Session["username"] = username;
                    return RedirectToAction("Index", "Receipt");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect Username or Password");
                }
                //string username = context.jct_login_emp.Where(x => x.empcode == authen.Username).Select(y => y.empcode).FirstOrDefault().ToString();
                //if (!username.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    byte[] password = context.jct_login_emp.Where(x => x.empcode == authen.Username).Select(y => y.new_pass).FirstOrDefault();
                //    string pass = Encoding.UTF8.GetString(password);
                //    if(pass.Equals(authen.Password))
                //    {
                //        var roles = context.jct_dak_role.Where(x => x.empcode == username).Select(m => m.Rolename).ToArray();
                //        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                //        serializeModel.UserId = username;
                //        serializeModel.FirstName = string.Empty;
                //        serializeModel.LastName = string.Empty;
                //        serializeModel.roles = roles;
                //        string userData = JsonConvert.SerializeObject(serializeModel);
                //        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                //             1,
                //            username,
                //             DateTime.Now,
                //             DateTime.Now.AddMinutes(15),
                //             false,
                //             userData);
                //        string encTicket = FormsAuthentication.Encrypt(authTicket);
                //        HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                //        Response.Cookies.Add(faCookie);
                //        Session["username"] = username;
                //        return RedirectToAction("Index","Receipt");
                //    }
                //}
                
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                Console.WriteLine(e.ToString());
            }
            finally
            {
                con.Close();
            }
            return View("Index");
        }


        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Authenticate", null);
        }

	}
}