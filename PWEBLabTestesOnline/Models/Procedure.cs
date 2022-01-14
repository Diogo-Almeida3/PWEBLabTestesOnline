using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Procedure
    {
        /* ----- Data ----- */
        public int ProcedureId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Description")]
        public string ProcedureDescription { get; set; }

        ///* ----- TypeAnalysisTests ----- */
        ////Each procedure corresponds to a analysis or test
        //[Required]
        //[Display(Name = "Type of Test")]
        //public int TypeAnalysisTestsId { get; set; }
        
        //public TypeAnalysisTests typeAnalysisTests { get; set; }
    }
}
