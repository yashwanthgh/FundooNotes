using ModelLayer.RegistrationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface ILoginRL
    {
        public Task<string> LoginUser(LoginUserModel loginUserModel);
        public Task<int> UpdatePassword(string email, string currentPassword, string newPassword);
        public Task<bool> ForgetPassword(string email);
        public Task<bool> ResetPassword(string token, string password);
    }
}
