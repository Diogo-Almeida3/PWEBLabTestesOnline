using System;
using System.ComponentModel.DataAnnotations;

namespace PWEBLabTestesOnline.Models
{
    public class AnalysisTests
    {

        /* ----- Data ----- */
        public int AnalysisTestsId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]

        [Display(Name = "Date")]
        public DateTime Date { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Location")]
        
        public string Location { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Result")]
        public string Result { get; set; }

        /* ----- Responsible ----- */


        [DataType(DataType.Text)]
        [Display(Name = "Techinician")]
        public string TechnicianName { get; set; }


        /* ----- Type ----- */
        [Required]
        [Display(Name = "Type Name")]
        public int TypeAnalysisTestsId { get; set; }

        public TypeAnalysisTests Type { get; set; }
    }
}
