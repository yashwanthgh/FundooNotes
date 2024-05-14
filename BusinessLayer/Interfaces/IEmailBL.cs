using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmailBL
    {
        public Task<bool> SendEmail(string to, string subject, string body);
    }
}
