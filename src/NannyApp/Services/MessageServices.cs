﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;

namespace NannyApp.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; private set; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new System.Net.Mail.MailAddress("mikey@starfantasygames.com", "Malik Gray");
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;

            // For SendGrid Username/Password
            var credentials = new System.Net.NetworkCredential(
                Options.SendGridUser,
                Options.SendGridPassword);

            // For SendGrid Api Key
            var apiKey = Options.SendGridKey;

            // Create a Web transport for sending email. (Using Username/Password)
            // var transportWeb = new SendGrid.Web(credentials);

            // Create a Web transport for sending email. (Using Api Key)
            var transportWeb = new SendGrid.Web(apiKey);

            // Send the email.
            if (transportWeb != null)
            {
                return transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
