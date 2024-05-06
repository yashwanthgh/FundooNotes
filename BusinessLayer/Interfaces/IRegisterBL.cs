using ModelLayer.RegistrationModel;
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
        public Task<ResponseModel<RegisterUserModel>> RegisterUser(RegisterUserModel registerUserModel);
    }
}
