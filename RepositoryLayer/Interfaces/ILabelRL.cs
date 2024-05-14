using ModelLayer.NotesModel;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface ILabelRL
    {
        public Task CreateLabel(CreateLabelModel label, int userId);
        public Task DeleteLabel(int labelId);
        public Task UpdateLabel(CreateLabelModel label, int labelId, int userId);
        public Task<IEnumerable<Label>> GetAllLabels();
        public Task<IEnumerable<object>> GetAllNotesbyId(int labelId);
    }
}
