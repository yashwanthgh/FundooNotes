using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Notes;
using ModelLayer.NotesModel;
using RepositoryLayer.Context;
using RepositoryLayer.Exceptions;
using RepositoryLayer.Interfaces;
using System.Data;


namespace RepositoryLayer.Services
{
    public class NotesServiceRL : INotesRL
    {
        private readonly DapperContext _context;
        private readonly IDistributedCache _cache;

        public NotesServiceRL(DapperContext context , IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<NoteResponse>> CreateNote(CreateNoteModel notes, int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Description", notes.Description, DbType.String);
            parameter.Add("Title", notes.Title, DbType.String);
            parameter.Add("Colour", notes.Colour, DbType.String);
            parameter.Add("IsArchived", notes.IsArchived, DbType.Boolean);
            parameter.Add("IsDeleted", notes.IsDeleted, DbType.Boolean);
            parameter.Add("UserId", userId, DbType.Int64);
            var selectQuery = @"SELECT * FROM Notes Where UserID = "+ userId;
            IEnumerable<NoteResponse> getNotes; 

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spInsertNote", parameter, commandType: CommandType.StoredProcedure);
                getNotes = await connection.QueryAsync<NoteResponse>(selectQuery, parameter);
            }
            if (getNotes != null)
                return getNotes.ToList();
            else
                throw new Exception("Empty no data found");
        }

        public async Task<NoteResponse> GetAllNotebyUserId(int NoteId, int UserId)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var note = await connection.QuerySingleOrDefaultAsync<NoteResponse>("spGetNoteByIdAndUserId", new { UserId, NoteId }, commandType: CommandType.StoredProcedure);

                    if (note == null)
                    {
                        throw new NotFoundException($"Note with NoteId '{NoteId}' does not exist for User with UserId '{UserId}'.");
                    }

                    return note;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task DeleteNote(int noteId, int userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("NoteId", noteId, DbType.Int64);
            parameter.Add("UserId", userId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spDeleteNoteByIdAndUserId", parameter, commandType: CommandType.StoredProcedure);

            }
            await _cache.RemoveAsync($"Notes_{userId}");
        }

        public async Task<NoteResponse> UpdateNote(int noteId, int userId, CreateNoteModel updatedNote)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Description", updatedNote.Description, DbType.String);
            parameter.Add("Title", updatedNote.Title, DbType.String);
            parameter.Add("Colour", updatedNote.Colour, DbType.String);
            parameter.Add("IsArchived", updatedNote.IsArchived, DbType.Boolean);
            parameter.Add("IsDeleted", updatedNote.IsDeleted, DbType.Boolean);
            parameter.Add("NoteId", noteId, DbType.Int64);
            parameter.Add("UserId", userId, DbType.Int64);
            var selectQuery = @"SELECT * FROM Notes Where NoteId = @NoteId";
            NoteResponse? getNotes;

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("spUpdateNote", parameter, commandType: CommandType.StoredProcedure);
                getNotes = await connection.QueryFirstOrDefaultAsync<NoteResponse>(selectQuery, parameter);
            }
            if (getNotes != null)
                return getNotes;
            else
                throw new Exception("Error occured");
        }

        public async Task<IEnumerable<NoteResponse>> GetAllNotes(int userId)
        { 
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>("spGetActiveNotesByUserId", new { UserId = userId }, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<NoteResponse>> GetAllArchivedNotes(int UserId)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var notes = await connection.QueryAsync<NoteResponse>("spGetArchivedNotes", new { UserId }, commandType: CommandType.StoredProcedure);
                    return notes.Reverse().ToList();
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
