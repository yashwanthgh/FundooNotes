using BusinessLayer.Interfaces;
using ModelLayer.NotesModel;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class LabelServiceBL : ILabelBL
    {
        private readonly ILabelRL _label;
        public LabelServiceBL(ILabelRL label)
        {
            _label = label;
        }

        public Task CreateLabel(CreateLabelModel label, int userId)
        {
            return _label.CreateLabel(label, userId);
        }

        public Task DeleteLabel(int labelId)
        {
            return _label.DeleteLabel(labelId);
        }

        public Task<IEnumerable<Label>> GetAllLabels()
        {
           return _label.GetAllLabels();
        }

        public Task<IEnumerable<object>> GetAllNotesbyId(int labelId)
        {
            return _label.GetAllNotesbyId(labelId);
        }

        public Task UpdateLabel(CreateLabelModel label, int labelId, int userId)
        {
            return _label.UpdateLabel(label, labelId, userId);  
        }
    }
}
