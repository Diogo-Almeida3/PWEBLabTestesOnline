using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBLabTestesOnline.Models
{
    public class Schedules
    {
        public int SchedulesId { get; set; }

        [Required]
        [Display(Name = "Appointment Time")]
        public DateTime AppointmentTime { get; set; }

        public Nullable<TestResult> Result { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required]
        [Display(Name ="Laboratory")]
        [ForeignKey("Laboratories")]
        public int LaboratoryId { get; set; }
        public Laboratories Laboratory { get; set; }


        [Required]
        [Display(Name = "Test Type")]
        [ForeignKey("TypeAnalysisTests")]
        public int TestTypeId { get; set; }
        public TypeAnalysisTests TestType { get; set; } 


        // Atribui automáticamente o cliente que cria a marcação
        [ForeignKey("ApplicationUser")]
        [Display(Name = "Client")]
        public string ClientId { get; set; }
        public ApplicationUser Client { get; set; }

        // O técnico 
        [ForeignKey("ApplicationUser")]
        [Display(Name = "Techinician")]
        public string TechinicianId { get; set; }
        public ApplicationUser Techinician { get; set; }
    }

    public enum TestResult
    {
        Positive,
        Negative,
        Inconclusive,
        Other,
        Unrealized
    }
}
