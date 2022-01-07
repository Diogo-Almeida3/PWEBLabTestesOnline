using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Laboratories
    {
        /* ----- Data ----- */
        public int LaboratoriesId { get; set;}

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Laboratory Name")]
        public string LaboratoriesName { get; set;}

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public int PhoneNumber { get; set; }

        /* ----- Owner - Manager ----- */
        [Required]
        [Display(Name = "Manager")]
        [ForeignKey("ApplicationUser")]
        public string ManagerId { get; set; }
        public ApplicationUser Manager { get; set;}

    }
}
