using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class TypeAnalysisTests
    {
        /* ----- Data ----- */
        public int TypeAnalysisTestsId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Type Name")]
        public string Name { get; set; }

        [ForeignKey("ApplicationUser")]
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

    }
}
