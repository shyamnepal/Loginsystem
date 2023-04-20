
using Azure.Core;
using Castle.Core.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MailSenderApp.Service
{
    public class EmailSender : IEmailSender
    {
        // out private configuration variables
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;

        public EmailSender(string host,int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }
        public void Send(string from, string to, string subject, string messageText)
        {
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL

            };
            client.SendMailAsync(
               new MailMessage(userName, to, subject, messageText) { IsBodyHtml = true }
               );

        }

    
        public void Send(IEnumerable<MailMessage> messages)
        {
            throw new NotImplementedException();
        }

        public void Send(MailMessage message)
        {
            throw new NotImplementedException();
        }

        //public async Task  SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    string fromMail = userName;
        //    string fromPassword = password;

        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress(fromMail);
        //    message.Subject = subject;
        //    message.To.Add(new MailAddress(email));
        //    message.Body = "<html><body> " + htmlMessage + " </body></html>";
        //    message.IsBodyHtml = true;

        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = port,
        //        Credentials = new NetworkCredential(fromMail, fromPassword),
        //        EnableSsl = enableSSL,
        //    };
        //    smtpClient.Send(message);
         
        //}
    }
}