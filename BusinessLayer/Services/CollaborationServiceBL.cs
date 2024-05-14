using BusinessLayer.Interfaces;
using ModelLayer.CollaborationMadel;
using ModelLayer.CollaborationModel;
using RepositoryLayer.Interfaces;


namespace BusinessLayer.Services
{
    public class CollaborationServiceBL : ICollaborationBL
    {
        private readonly ICollaborationRL _collaboration;

        public CollaborationServiceBL(ICollaborationRL collaboration)
        {
            _collaboration = collaboration;
        }

        public Task<bool> AddCollaborator(int noteId, CollaborationCreateMode model, int userId)
        {
           return _collaboration.AddCollaborator(noteId, model, userId);
        }

        public Task<IEnumerable<CollaborationInfoModel>> GetCollaboration()
        {
            return _collaboration.GetCollaboration();
        }

        public Task RemoveCollaborator(int CollaborationId)
        {
            return _collaboration.RemoveCollaborator(CollaborationId);
        }
    }
}
