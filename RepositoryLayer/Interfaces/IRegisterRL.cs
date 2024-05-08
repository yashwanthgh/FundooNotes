using ModelLayer.RegistrationModel;
using ModelLayer.Response;

namespace RepositoryLayer.Interfaces
{
    public interface IRegisterRL
    {
        public Task<bool> RegisterUser(RegisterUserModel registerUserModel);
    }
}
