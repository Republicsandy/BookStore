using Experimental.System.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CommonLayer.Models
{
    public class MSMQModel
    {
        MessageQueue messageQueue = new MessageQueue();
        public void sendData2Queue(string token)
        {
            messageQueue.Path = @".\private$\Messages";
            if (!MessageQueue.Exists(messageQueue.Path))
            {
                MessageQueue.Create(messageQueue.Path);
            }
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            messageQueue.ReceiveCompleted += MessageQ_ReceiveCompleted;
            messageQueue.Send(token);
            messageQueue.BeginReceive();
            messageQueue.Close();
        }

        private void MessageQ_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var msg = messageQueue.EndReceive(e.AsyncResult);
            string token = msg.Body.ToString();
            string subject = "Bookstore Reset Link";
            string Body = "Reset Password for Book Store : " + token;
            var SMTP = new SmtpClient("Smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("helpindia82@gmail.com", "erpkdqhgjefkzzxn"),
                EnableSsl = true
            };
            SMTP.Send("helpindia82@gmail.com", "helpindia82@gmail.com", subject, Body);
            messageQueue.BeginReceive();
        }
    }
}
