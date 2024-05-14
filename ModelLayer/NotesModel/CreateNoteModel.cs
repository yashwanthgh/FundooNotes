using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Notes
{
    public class CreateNoteModel
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
