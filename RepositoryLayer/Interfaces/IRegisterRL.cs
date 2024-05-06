using ModelLayer.RegistrationModel;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Interfaces
{
    public interface IRegisterRL
    {
        public Task<ResponseModel<RegisterUserModel>> RegisterUser(RegisterUserModel registerUserModel);
    }
}
