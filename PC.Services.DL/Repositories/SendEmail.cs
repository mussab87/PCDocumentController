using PC.Services.Core.EmailModel;
using PC.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.Repositories
{
    public class SendEmail : ISendEmail
    {
        async Task<bool> ISendEmail.SendEmail(EmailInfo emailInfo)
        {
            try
            {
                MailMessage email = new MailMessage();
                //SmtpClient client = new SmtpClient(emailInfo.SmtpClient);
                email.To.Add(emailInfo.To);
                //mail.To.Add("Secondry@gmail.com");
                email.From = new MailAddress(emailInfo.From);
                email.Subject = emailInfo.Subject;
                email.Body = emailInfo.messageBody;
                email.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(emailInfo.SmtpClient, emailInfo.SmtpPort);
                // smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.UseDefaultCredentials = emailInfo.UseDefaultCredentials;
                smtp.EnableSsl = emailInfo.EnableSsl;
                smtp.Credentials = new System.Net.NetworkCredential(emailInfo.From, emailInfo.SmtpCredentials);
                // smtp.Port = 587;
                //Or your Smtp Email ID and Password
                await smtp.SendMailAsync(email);

                return Task.CompletedTask.IsCompleted;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
