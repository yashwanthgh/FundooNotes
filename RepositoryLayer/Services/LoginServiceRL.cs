using ModelLayer.RegistrationModel;
using RepositoryLayer.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using RepositoryLayer.Entities;
using RepositoryLayer.Context;
using RepositoryLayer.Exceptions;

namespace RepositoryLayer.Services
{
    public class LoginServiceRL : ILoginRL
    {
        private readonly DapperContext _context;
        private readonly IAuthServiceRL _authService;

        public LoginServiceRL(DapperContext context, IAuthServiceRL authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<string> LoginUser(LoginUserModel loginUserModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", loginUserModel.Email);

            string query = @"SELECT * FROM Users WHERE Email = @Email;";


            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstAsync<Register>(query, parameters);

                if (user == null)
                {
                    throw new UserNotFoundException($"User with email '{loginUserModel.Email}' not found.");
                }

                if (!BCrypt.Net.BCrypt.Verify(loginUserModel.Password, user.Password))
                {
                    throw new InvalidPasswordException($"User with Password '{loginUserModel.Password}' not Found.");
                }

                // If password enterd from user and password in db match then generate Token 
                var token = _authService.GenerateJwtToken(user);
                return token;
            }
        }
    }
}
