using ModelLayer.RegistrationModel;
using RepositoryLayer.Interfaces;
using Dapper;
using RepositoryLayer.Entities;
using RepositoryLayer.Context;
using RepositoryLayer.Exceptions;
using System.Data;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using ModelLayer.EmailModel;

namespace RepositoryLayer.Services
{
    public class LoginServiceRL : ILoginRL
    {
        private readonly DapperContext _context;
        private readonly IAuthServiceRL _authService;
        private readonly EmailSettingModel _emailSetting;

        public LoginServiceRL(DapperContext context, IAuthServiceRL authService, EmailSettingModel emailSetting)
        {
            _context = context;
            _authService = authService;
            _emailSetting = emailSetting;
        }

        public async Task<string> LoginUser(LoginUserModel loginUserModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", loginUserModel.Email);

            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstAsync<Register>("spLoginUsingEmail", parameters, commandType: CommandType.StoredProcedure);

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

        private static bool IsPasswordValid(string? password)
        {
            var pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])(?=.*[0-9]).{8,15}$";
            if (password == null) return false;
            return Regex.IsMatch(password, pattern);
        }

        public async Task<int> UpdatePassword(string email, string currentPassword, string newPassword)
        {
            if (!IsPasswordValid(newPassword))
            {
                throw new InvalidPasswordFormateException("Invalid Password formate!");
            }
            using (var connection = _context.CreateConnection())
            {
                var currentPasswordFromDatabase = connection.QueryFirstOrDefault<string>("spGetPasswordForEmail", new { Email = email }, commandType: CommandType.StoredProcedure);
                if (currentPasswordFromDatabase == null)
                {
                    throw new InvalidEmailException($"Entered email {email} don't exists");
                }

                var isPasswordCurrentPasswordMatching = BCrypt.Net.BCrypt.Verify(currentPassword, currentPasswordFromDatabase);
                if (!isPasswordCurrentPasswordMatching)
                {
                    throw new InvalidPasswordException($"Current Password is Invalid");
                }

                var Parameter = new DynamicParameters();

                Parameter.Add("Email", email, DbType.String);
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                Parameter.Add("Password", hashPassword, DbType.String);
                return await connection.ExecuteAsync("spUpdatePassword", Parameter, commandType: CommandType.StoredProcedure);
            }
        }

        // To geterate the token 
        private static string GenerateToken()
        {
            const int tokenLength = 32;
            var randomBytes = new byte[tokenLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).Replace("/", "-").Replace("+", "_").Replace("=", "");
        }

        // Logic to send link to the users mail
        private async Task<bool> SendResetLinkEmail(string email, string token)
        {
            var mailMessage = new MailMessage();
            var senderEmailID = _emailSetting.Username;
            if (senderEmailID != null)
            {
                mailMessage.From = new MailAddress(senderEmailID);
            }
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Subject = "Password Reset for Your Account";

            // string resetLink = $"http://localhost:{7229}/reset-password?token={token}";
            // string message = $"Click here to reset your password: {resetLink}";
            string message = $" Token to reset your password: {token}";
            mailMessage.Body = message;

            using (var smtpClient = new SmtpClient(_emailSetting.Server, _emailSetting.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSetting.Username, _emailSetting.Password);
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
            return true;
        }

        public async Task<bool> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidEmailException("Email is required!");
            }

            using var connection = _context.CreateConnection();
            var emailExists = connection.QueryFirstOrDefault<bool>("spToCheckEmailIsNotDuplicate", new { Email = email }, commandType: CommandType.StoredProcedure);
            if (!emailExists)
            {
                throw new InvalidEmailException("Email Dosn't exists, Register first!");
            }
            // Getting random token
            string token = GenerateToken();
            int getIdOfUser = connection.QueryFirstOrDefault<int>("spGetIdForEmail", new { Email = email }, commandType: CommandType.StoredProcedure);

            // Check if the token already exists for the user
            var tokenExists = connection.QueryFirstOrDefault<bool>("spCheckTokenExistsForUser", new { Id = getIdOfUser }, commandType: CommandType.StoredProcedure);

            // If the token exists, update it; otherwise, insert a new token
            if (tokenExists)
            {
                connection.Execute("spUpdateToken", new { Id = getIdOfUser, Token = token, ExpiryDate = DateTime.UtcNow.AddMinutes(15) }, commandType: CommandType.StoredProcedure);
            }
            else
            {
                connection.Execute("spAddToken", new { Id = getIdOfUser, Token = token, ExpiryDate = DateTime.UtcNow.AddMinutes(15) }, commandType: CommandType.StoredProcedure);
            }
            return await SendResetLinkEmail(email, token);
        }

        public async Task<bool> ResetPassword(string token, string password)
        {
            if (!IsPasswordValid(password))
            {
                throw new InvalidPasswordFormateException("Invalid Password formate!");
            }
            
            if(token == null)
            {
                throw new InvalidTokenException("Invalid Token!");
            }

            using var connection = _context.CreateConnection();
            var tokenInfo = connection.QueryFirstOrDefault("spGetTokenInfo", new { Token = token }, commandType: CommandType.StoredProcedure);
            if (tokenInfo == null)
            {
                throw new InvalidTokenException("Invalid token");
            }

            var expiryDate = tokenInfo.ExpiryDate;
            if (expiryDate < DateTime.UtcNow)
            {
                throw new TokenExpiredException("Token has expired!");
            }
            // Hashing the password using BCrypt 
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

            await connection.ExecuteAsync("spUpdatePasswordUsingId", new { tokenInfo.Id, Password = hashPassword }, commandType: CommandType.StoredProcedure);

            return true;
        }
    }
}
