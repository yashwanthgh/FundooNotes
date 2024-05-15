using ModelLayer.CollaborationMadel;
using ModelLayer.CollaborationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface ICollaborationRL
    {
        public Task<bool> AddCollaborator(int noteId, CollaborationCreateModel model, int userId);
        public Task RemoveCollaborator(int CollaborationId);
        public Task<IEnumerable<CollaborationInfoModel>> GetCollaboration();
    }
}
