using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.CollaborationModel
{
    public class CollaborationInfoModel
    {
        public int CollaborationId { get; set; }
        public int UserId { get; set; }
        public int NoteId { get; set; }
        public string? CollaborationEmail { get; set; }
    }
}
