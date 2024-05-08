using Dapper;
using ModelLayer.RegistrationModel;
using ModelLayer.Response;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Exceptions;
using RepositoryLayer.Interfaces;
using System.Data;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Services
{
    public class RegisterServiceRL : IRegisterRL
    {
        private readonly DapperContext _context;

        public RegisterServiceRL(DapperContext context)
        {
            _context = context;
        }

        // Validating the Email
        private static bool IsEmailValid(string? email)
        {
            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if(email == null) return false;
            return Regex.IsMatch(email, pattern);
        }

        // Validating the Password
        private static bool IsPasswordValid(string? password)
        {
            var pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])(?=.*[0-9]).{8,15}$";
            if(password == null) return false;  
            return Regex.IsMatch(password, pattern);
        }

        public async Task<bool> RegisterUser(RegisterUserModel registerUserModel)
        {
            // If the email is not in proper formate 
            if(!IsEmailValid(registerUserModel.Email))
            {
                throw new InvalidEmailFormateException("Invalid email formate");
            }

            if (!IsPasswordValid(registerUserModel.Password))
            {
                throw new InvalidPasswordFormateException("Invalid password formate");
            }

            /*
             * var newUserEmail = registerUserModel.Email;
             * var toCheckEmailIsNotDuplicate = @"SELECT COUNT(*) FROM Users WHERE Email = " + newUserEmail;
             * 
             *  ++ After making a connection
             * if(toCheckEmailIsNotDuplicate > 0) throw new DuplicateEmailException("Email alredy exists!");
             * 
             * */

            // Getting Email to check if it exists
            var gettingEmailFromRegisteringUser = new DynamicParameters();
            gettingEmailFromRegisteringUser.Add("Email",  registerUserModel.Email, DbType.String);

            // If the count is more than 0 then email exists
            var toCheckEmailIsNotDuplicate = @"SELECT COUNT(*) FROM Users WHERE Email = @Email";

            // To Add parameters to the query
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", registerUserModel.FirstName, DbType.String);
            parameters.Add("LastName", registerUserModel.LastName, DbType.String);
            parameters.Add("Email", registerUserModel.Email, DbType.String);

            // Hashing the password using BCrypt 
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(registerUserModel.Password);
            parameters.Add("Password", hashPassword, DbType.String);

            var query = @"INSERT INTO Users (FirstName, LastName, Email, PAssword)
                           VALUES (@FirstName, @LastName, @Email, @Password)";

            using(var connection = _context.CreateConnection())
            {
                // Returns true if the email exists
                var emailExists = connection.QueryFirstOrDefault<bool>(toCheckEmailIsNotDuplicate, gettingEmailFromRegisteringUser);

                if (emailExists)
                {
                    throw new DuplicateEmailException("Email alredy exists!");
                }

                await connection.ExecuteAsync(query, parameters);
                return true;
            }
        }
    }
}
