using DakManSys.Models;
using DakManSys.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
namespace DakManSys.MailFunctionality
{
    public class MailSend
    {

        public string sendMail(BulkMailModel model)
        {
            //string to = model.Jct_Dak_Register.MailTo;
            string to = "appanc@jctltd.com";
            string from = "noreply@jctltd.com";
            string bcc = "manishk@jctltd.com,appanc@jctltd.com,aslam@jctltd.com";
            //string cc = model.Jct_Dak_Register.MailCC;
            //string subject = model.Jct_Dak_Register.SUBJECT;


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h3>A Dak addressed to you has been recieved by the Admin Department.The Detail of the given Dak is as follows:</h3><br/>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>");
            sb.AppendLine("<strong> Total Dak Number : </strong> ");
            sb.AppendLine("</td>");
            sb.AppendLine("<td>");
            sb.AppendLine(model.Count.ToString());
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

            MailMessage mail = new MailMessage();

            //if (!string.IsNullOrEmpty(cc))
            //{
            //    if (cc.Contains(","))
            //    {
            //        string[] ccs = cc.Split(',');
            //        for (int i = 0; i < ccs.Length - 1; i++)
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
            mail.To.Add(new MailAddress(to));
            //mail.Subject = subject;
            mail.Body = sb.ToString();
            mail.IsBodyHtml = true;
            SmtpClient SmtpMail = new SmtpClient("exchange2k7");
            SmtpMail.Send(mail);
            return "a";
        }

    }
}