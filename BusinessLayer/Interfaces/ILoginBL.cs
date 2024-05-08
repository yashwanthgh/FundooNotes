using ModelLayer.RegistrationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILoginBL
    {
        public Task<string> UserLogin(LoginUserModel loginUserModel);
    }
}
