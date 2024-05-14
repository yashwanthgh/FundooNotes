using Dapper;
using ModelLayer.NotesModel;
using RepositoryLayer.Context;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class LabelServiceRL : ILabelRL
    {
        private readonly DapperContext _context;
        public LabelServiceRL(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateLabel(CreateLabelModel label, int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelName", label.LabelName, DbType.String);
            parameter.Add("NoteId", label.NoteId, DbType.Int64);
            parameter.Add("UserId", userId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spCreateLabels", parameter, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteLabel(int labelId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", labelId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spDeleteLabel", parameter, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Label>> GetAllLabels()
        {
            var query = "SELECT * FROM Labels;";

            using (var connection = _context.CreateConnection())
            {
                var Label = await connection.QueryAsync<Label>(query);
                return Label.ToList();
            }
        }

        public async Task<IEnumerable<object>> GetAllNotesbyId(int labelId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", labelId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                var Label = await connection.QueryAsync<object>("spGetNotesByLabelId", parameter, commandType: CommandType.StoredProcedure);
                return Label.ToList();
            }
        }

        public async Task UpdateLabel(CreateLabelModel label, int labelId, int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("LabelId", labelId, DbType.Int64);
            parameter.Add("LabelName", label.LabelName, DbType.String);
            parameter.Add("NoteId", label.NoteId, DbType.Int64);
            parameter.Add("UserId", userId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spUpdateLabel", parameter, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
