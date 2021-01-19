using Crm6.App_Code;
using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using Models;
using System.Net.Mail;
using System.Net.Mime;

namespace Helpers
{
    public class SendGridHelper
    {

        /// <summary>
        /// This function send the email using sendgrid API
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendEmail(SendEmailRequest request)
        {
            // send to users
            if (request.UserRecipients != null)
                foreach (var recipient in request.UserRecipients)
                {
                    SendEmail(recipient, request);
                }

            // send to contact recipient
            if (request.ContactRecipients != null)
                foreach (var recipient in request.ContactRecipients)
                {
                    SendEmail(recipient, request);
                }

            // TODO: direct email to non-CRM User or Contact ??

            // send to other recipient 
            if (request.OtherRecipients != null)
                foreach (var recipient in request.OtherRecipients)
                {
                    SendEmail(recipient, request);
                }

            return true;
        }


        private void SendEmail(Recipient recipient, SendEmailRequest request)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                // To
                mailMsg.To.Add(new MailAddress(recipient.EmailAddress, recipient.EmailAddress));
                // From
                mailMsg.From = new MailAddress(request.Sender.EmailAddress, request.Sender.Name);

                if (!string.IsNullOrEmpty(request.ReplyToEmail))
                {
                    //mailMsg.ReplyToList = new MailAddressCo(request.ReplyToEmail);
                    mailMsg.ReplyToList.Add( request.ReplyToEmail ); 
                } 

                // Subject and multipart/alternative Body
                mailMsg.Subject = request.Subject;
                string html = request.HtmlBody;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_4cc10694f17920369e1b030c642df6c7@azure.com", "Sendgrid#1350");
                smtpClient.Credentials = credentials;

                if (request.Attachments != null)
                {
                    foreach (var att in request.Attachments)
                    {
                        mailMsg.Attachments.Add(att);
                    }
                }
               
                smtpClient.Send(mailMsg);
            }
            catch (Exception)
            {
            }


            //string apiKey = "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";
            //var client = new SendGridClient(apiKey);
            //var from = new SendGrid.Helpers.Mail.EmailAddress(request.Sender.EmailAddress, request.Sender.Name);
            //var to = new SendGrid.Helpers.Mail.EmailAddress(recipient.EmailAddress);
            //var subject = request.Subject;
            //if (!string.IsNullOrEmpty(recipient.Name))
            //{
            //    to.Name = recipient.Name;
            //}
            //var htmlContent = request.HtmlBody;
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            //var response = client.SendEmailAsync(msg);

            // add a record to email table
        }




    }
}