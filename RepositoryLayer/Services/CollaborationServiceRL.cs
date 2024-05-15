using Dapper;
using ModelLayer.CollaborationMadel;
using ModelLayer.CollaborationModel;
using ModelLayer.EmailModel;
using RepositoryLayer.Context;
using RepositoryLayer.Exceptions;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class CollaborationServiceRL : ICollaborationRL
    {
        private readonly DapperContext _context;
        private readonly IEmailRL _emailService;

        public CollaborationServiceRL(DapperContext context, IEmailRL emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> AddCollaborator(int noteId, CollaborationCreateModel model, int userId)
        {
            var parameter = new DynamicParameters();

            parameter.Add("NoteId", noteId, DbType.Int64);
            parameter.Add("UserId", userId, DbType.Int64);

            if (!IsEmailValid(model.Email))
            {
                throw new InvalidEmailFormateException("Invalid Email Format");
            }
            parameter.Add("CollaborationEmail", model.Email, DbType.String);

            var emailExistparameter = new { CollabEmail = model.Email };
            using (var connection = _context.CreateConnection())
            {
                int emailcount = await connection.ExecuteAsync("spCheckEmailExistence", new { CollaberationEmail = model.Email }, commandType: CommandType.StoredProcedure);
                if (emailcount == 0)
                {
                    throw new NotFoundException($"Collaborator with the Email '{model.Email}'is not a registered user");
                }

                await connection.ExecuteAsync("spCreateCollaboration", parameter);
                var emailBody = $"You have been added as a collaborator.";
                await _emailService.SendEmail(model.Email, "Added as Collaborator", emailBody);

            }
            return true;
        }

        public async Task<IEnumerable<CollaborationInfoModel>> GetCollaboration()
        {
            using (var connection = _context.CreateConnection())
            {
                var collaborations = await connection.QueryAsync<CollaborationInfoModel>("spGetAllCollaborations", commandType: CommandType.StoredProcedure);
                return collaborations;
            }
        }

        public async Task RemoveCollaborator(int collaborationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("CollaborationId", collaborationId, DbType.Int64);
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("spDeleteCollaborationById", parameter, commandType: CommandType.StoredProcedure);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool IsEmailValid(string? email)
        {
            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (email == null) return false;
            return Regex.IsMatch(email, pattern);
        }
    }
}
