using ModelLayer.Notes;
using ModelLayer.NotesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface INotesBL
    {
        public Task<IEnumerable<NoteResponse>> CreateNote(CreateNoteModel notes, int userId);
        public Task<NoteResponse> GetAllNotebyuserId(int NoteId, int userId);
        public Task<IEnumerable<NoteResponse>> GetAllNotes(int userid);
        public Task<NoteResponse> UpdateNote(int noteId, int userId, CreateNoteModel updatedNote);
        public Task DeleteNote(int noteId, int userId);
        Task<IEnumerable<NoteResponse>> GetAllArchivedNotes(int UserId);
    }
}
