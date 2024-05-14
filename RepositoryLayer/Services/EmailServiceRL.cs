using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.EmailModel;
using RepositoryLayer.Exceptions;

namespace RepositoryLayer.Services
{
    public class EmailServiceRL : IEmailRL
    {
        private readonly EmailSettingModel _emailSetting;

        public EmailServiceRL(EmailSettingModel emailSetting)
        {
            _emailSetting = emailSetting;
        }

        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_emailSetting.Server, _emailSetting.Port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailSetting.Username, _emailSetting.Password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSetting.Username),
                        Subject = subject,
                        Body = body
                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (SmtpException ex)
            {
                throw new EmailSendingException($"failed to send email: SMTP error {ex.Message}");
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new EmailSendingException($"Invalid operation occurred while sending email. {invalidOpEx.Message}");
            }
            catch 
            {
                return false;
            }
        }
    }
}
