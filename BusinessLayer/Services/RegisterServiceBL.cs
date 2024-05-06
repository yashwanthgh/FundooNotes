using BusinessLayer.Interfaces;
using ModelLayer.RegistrationModel;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class RegisterServiceBL : IRegisterBL
    {
        private readonly IRegisterRL _regester;

        public RegisterServiceBL(IRegisterRL regester)
        {
            _regester = regester;
        }

        public async Task<ResponseModel<RegisterUserModel>> RegisterUser(RegisterUserModel registerUserModel)
        {
            return await _regester.RegisterUser(registerUserModel);
        }
    }
}
