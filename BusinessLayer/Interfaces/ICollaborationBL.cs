using ModelLayer.CollaborationMadel;
using ModelLayer.CollaborationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICollaborationBL
    {
        public Task<bool> AddCollaborator(int noteId, CollaborationCreateModel model, int userId);
        public Task RemoveCollaborator(int CollaborationId);
        public Task<IEnumerable<CollaborationInfoModel>> GetCollaboration();
    }
}
