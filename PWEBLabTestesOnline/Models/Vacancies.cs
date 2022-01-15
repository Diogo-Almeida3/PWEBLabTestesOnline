using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Vacancies
    {
        [Key]
        public int VacanciesId { get; set; }

        [Required(ErrorMessage = "Laboratory is necessary")]
        [ForeignKey("Laboratories")]
        [Display(Name = "Laboratory Name")]
        public int LaboratoryId { get; set; }
        public Laboratories Laboratory { get; set; }

        [Required(ErrorMessage = "Test type is necessary")]
        [ForeignKey("Type")]
        [Display(Name = "Type")]
        public int TypeAnalysisTestsId { get; set; }
        public TypeAnalysisTests Type { get; set; }

        [Required]
        [Display(Name = "Daily Limit")]
        [DataType(DataType.Text)]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 1")]
        public int DailyLimit { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Opening { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Enclosure { get; set; }

        
        [Required]
        [DataType(DataType.Text)]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 1")]
        [Display(Name = "Duration in minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Check list is necessary")]
        [ForeignKey("Checklist")]
        public int CurrentChecklistId { get; set; }
        public Checklist CurrentChecklist { get; set; }
    }
}
