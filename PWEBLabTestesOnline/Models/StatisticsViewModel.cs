using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PWEBLabTestesOnline.Models
{
    public class StatisticsViewModel
    {
        public AllTests all { get; set; }

        public DateTime FilterDay { get; set; }
        public AllTests OnDay { get; set; }


        public DateTime FilterWeekDay1 { get; set; }
        public DateTime FilterWeekDay2 { get; set; }
        public AllTests OnWeek { get; set; }

        public DateTime Month { get; set; }
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
