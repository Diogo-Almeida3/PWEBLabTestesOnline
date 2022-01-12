using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Vacancies
    {
        [Key]
        public int VacanciesId { get; set; }

        [Required]
        [ForeignKey("Laboratories")]
        [Display(Name = "Laboratory Name")]
        public int LaboratoryId { get; set; }
        public Laboratories Laboratory { get; set; }

        [Required]
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

        //TODO: Colocar duração do teste
    }
}
