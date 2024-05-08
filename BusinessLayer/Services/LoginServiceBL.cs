using BusinessLayer.Interfaces;
using ModelLayer.RegistrationModel;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class LoginServiceBL : ILoginBL
    {
        private readonly ILoginRL _login;

        public LoginServiceBL(ILoginRL login)
        {
            _login = login;
        }

        public Task<string> UserLogin(LoginUserModel loginUserModel)
        {
            return _login.LoginUser(loginUserModel);
        }
    }
}
