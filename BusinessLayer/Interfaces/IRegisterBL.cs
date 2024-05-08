using ModelLayer.RegistrationModel;
using ModelLayer.Response;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IRegisterBL
    {
        public Task<bool> RegisterUser(RegisterUserModel registerUserModel);
    }
}
