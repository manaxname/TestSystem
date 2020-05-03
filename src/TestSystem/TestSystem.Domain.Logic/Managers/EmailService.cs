using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using TestSystem.Domain.Logic.Interfaces;
using MimeKit.Text;

namespace TestSystem.Domain.Logic.Managers
{
    public class EmailService : IEmailService
    {
        private static readonly string _xoauth2 = "XOAUTH2";

        public async Task SendEmailAsync(string senderName, string senderEmail, string senderEmailPassword,
            string smtpHost, int smtpPort, string recipientEmail, string subject, string message)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            mimeMessage.To.Add(new MailboxAddress("", recipientEmail));
            mimeMessage.Subject = subject;

            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message,
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(smtpHost, smtpPort, false);

                client.AuthenticationMechanisms.Remove(_xoauth2);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(senderEmail, senderEmailPassword);

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}