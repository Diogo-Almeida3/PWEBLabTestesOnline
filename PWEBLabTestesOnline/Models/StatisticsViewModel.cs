using System.Collections.Generic;

namespace PWEBLabTestesOnline.Models
{
    public class StatisticsViewModel
    {
        public AllTests all { get; set; }

        public ICollection<Schedules> TestsOnDay { get; set; }
        public AllTests OnDay { get; set; }


        public ICollection<Schedules> TestsOnWeek { get; set; }
        public AllTests OnWeek { get; set; }

        public ICollection<Schedules> TestsOnMonth { get; set; }
        public AllTests OnMonth { get; set; }
    }

    public class AllTests
    {
        public int TotalTests { get; set; }
        public int TotalPositiveTests { get; set; }
        public int TotalNegativeTests { get; set; }
        public int TotalInconclusiveTests { get; set; }
    }
}
