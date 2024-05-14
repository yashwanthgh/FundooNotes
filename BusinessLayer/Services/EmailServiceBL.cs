using BusinessLayer.Interfaces;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EmailServiceBL : IEmailBL
    {
        private readonly IEmailRL _email;

        public EmailServiceBL(IEmailRL email)
        {
            _email = email;
        }

        public Task<bool> SendEmail(string to, string subject, string body)
        {
            return _email.SendEmail(to, subject, body);
        }
    }
}
