using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net.Mail;

namespace Tipset.Helpers
{
    public static class SendEmail 
    {
        public static void SendEmail_SMTP(string strFromEmail,
                                    string strFromName,
                                    string strToEmail,
                                    string strToName,
                                    string strSubject,
                                    string strBody)
        {
            // Create the message body
            MailMessage msgMail = new MailMessage();
            msgMail.To.Add(new MailAddress(strToEmail, strToName));
            msgMail.From = new MailAddress(strFromEmail, strFromName);
            msgMail.Subject = strSubject;
            msgMail.Body = strBody;
            msgMail.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("relay-hosting.secureserver.net", 25);
            smtpClient.Send(msgMail);
        }
    }
}
