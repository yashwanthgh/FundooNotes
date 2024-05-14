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

        public async Task<bool> ForgetPassword(string email)
        {
           return await _login.ForgetPassword(email);
        }

        public async Task<int> UpdatePassword(string email, string currentPassword, string newPassword)
        {
           return await _login.UpdatePassword(email, currentPassword, newPassword);
        }

        public  async Task<string> UserLogin(LoginUserModel loginUserModel)
        {
            return await _login.LoginUser(loginUserModel);
        }

        public async Task<bool> ResetPassword(string token, string password)
        {
            return await _login.ResetPassword(token, password);
        }
    }
}
