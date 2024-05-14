using ModelLayer.Notes;
using ModelLayer.NotesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        public Task CreateNote(CreateNoteModel notes, int userId);
        public Task<NoteResponse> GetAllNotebyUserId(int NoteId, int userId);
        public Task<IEnumerable<NoteResponse>> GetAllNotes(int userid);
        public Task UpdateNote(int noteId, int userId, CreateNoteModel updatedNote);
        public Task DeleteNote(int noteId, int userId);
        public Task<IEnumerable<NoteResponse>> GetAllArchivedNotes(int UserId);
    }
}
