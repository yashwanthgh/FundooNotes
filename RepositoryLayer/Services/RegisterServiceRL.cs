using Confluent.Kafka;
using Dapper;
using ModelLayer.RegistrationModel;
using ModelLayer.Response;
using Newtonsoft.Json;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Exceptions;
using RepositoryLayer.Helper;
using RepositoryLayer.Interfaces;
using System.Data;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Services
{
    public class RegisterServiceRL : IRegisterRL
    {
        private readonly DapperContext _context;
        private readonly IEmailRL _email;

        public RegisterServiceRL(DapperContext context, IEmailRL email)
        {
            _context = context;
            _email = email;
        }

        // Validating the Email
        private static bool IsEmailValid(string? email)
        {
            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (email == null) return false;
            return Regex.IsMatch(email, pattern);
        }

        // Validating the Password
        private static bool IsPasswordValid(string? password)
        {
            var pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])(?=.*[0-9]).{8,15}$";
            if (password == null) return false;
            return Regex.IsMatch(password, pattern);
        }

        public async Task<bool> RegisterUser(RegisterUserModel registerUserModel)
        {
            // If the email is not in proper formate 
            if (!IsEmailValid(registerUserModel.Email))
            {
                throw new InvalidEmailFormateException("Invalid email formate");
            }

            if (!IsPasswordValid(registerUserModel.Password))
            {
                throw new InvalidPasswordFormateException("Invalid password formate");
            }

            // Getting Email to check if it exists
            var gettingEmailFromRegisteringUser = new DynamicParameters();
            gettingEmailFromRegisteringUser.Add("Email", registerUserModel.Email, DbType.String);

            // To Add parameters to the query
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", registerUserModel.FirstName, DbType.String);
            parameters.Add("LastName", registerUserModel.LastName, DbType.String);
            parameters.Add("Email", registerUserModel.Email, DbType.String);

            // Hashing the password using BCrypt 
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(registerUserModel.Password);
            parameters.Add("Password", hashPassword, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                // Returns true if the email exists
                var emailExists = connection.QueryFirstOrDefault<bool>("spToCheckEmailIsNotDuplicate", gettingEmailFromRegisteringUser, commandType: CommandType.StoredProcedure);

                if (emailExists)
                {
                    throw new DuplicateEmailException("Email alredy exists!");
                }

                await connection.ExecuteAsync("spRegisterUser", parameters, commandType: CommandType.StoredProcedure);

                var registrationDetailsForPublishing = new KafkaDetailsPublish(registerUserModel);

                // Serialize registration details to a JSON string
                var registrationDetailsJson = JsonConvert.SerializeObject(registrationDetailsForPublishing);

                // Get Kafka producer configuration
                var producerConfig = KafkaProducerConfig.GetProducerConfig();

                // Create a Kafka producer
                using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
                {
                    try
                    {
                        // Publish registration details to Kafka topic
                        await producer.ProduceAsync("Registration-topic", new Message<Null, string> { Value = registrationDetailsJson });
                        Console.WriteLine("Registration details published to Kafka topic.");
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Failed to publish registration details to Kafka topic: {e.Error.Reason}");
                    }
                }

                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092", // Kafka broker(s) address
                    GroupId = "my-consumer-group", // Consumer group ID
                    AutoOffsetReset = AutoOffsetReset.Earliest // Reset offset to the earliest message in case no offset is committed
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    consumer.Subscribe("Registration-topic"); // Subscribe to Kafka topic

                    try
                    {
                        var consumeResult = consumer.Consume(); // Consume message
                        await _email.SendEmail(registerUserModel.Email, "Register Successful", $"Welcome to FundooNotes {registerUserModel.FirstName}");
                        Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
                return true;
            }
        }
    }
}
