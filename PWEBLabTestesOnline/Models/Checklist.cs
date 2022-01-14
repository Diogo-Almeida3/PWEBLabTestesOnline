using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Checklist
    {
        public int ChecklistId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [ForeignKey("ApplicationUser")]
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public ICollection<Procedure> Procedures { get; set; }
    }
}
