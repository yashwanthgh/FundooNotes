using BusinessLayer.Interfaces;
using ModelLayer.Notes;
using ModelLayer.NotesModel;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class NotesServiceBL : INotesBL
    {
        private readonly INotesRL _notes;

        public NotesServiceBL(INotesRL notes)
        {
            _notes = notes;
        }

        public Task CreateNote(CreateNoteModel notes, int userId)
        {
           return _notes.CreateNote(notes, userId);
        }

        public Task DeleteNote(int noteId, int userId)
        {
            return (_notes.DeleteNote(noteId, userId));
        }

        public Task<IEnumerable<NoteResponse>> GetAllArchivedNotes(int UserId)
        {
            return _notes.GetAllArchivedNotes(UserId);
        }

        public Task<NoteResponse> GetAllNotebyuserId(int NoteId, int userId)
        {
            return _notes.GetAllNotebyUserId(NoteId, userId);
        }

        public Task<IEnumerable<NoteResponse>> GetAllNotes(int userId)
        {
            return _notes.GetAllNotes(userId);
        }

        public Task UpdateNote(int noteId, int userId, CreateNoteModel updatedNote)
        {
           return _notes.UpdateNote(noteId, userId, updatedNote);
        }
    }
}
