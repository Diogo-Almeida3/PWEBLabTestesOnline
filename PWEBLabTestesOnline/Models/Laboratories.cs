using System.ComponentModel.DataAnnotations;

namespace PWEBLabTestesOnline.Models
{
    public class Laboratories
    {
        /* ----- Data ----- */
        public int LaboratoriesId { get; set;}

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Laboratorie Name")]
        public string LaboratoriesName { get; set;}

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        /* ----- Owner - Manager ----- */
        [Required]
        public ApplicationUser Manager { get; set;}

    }
}
