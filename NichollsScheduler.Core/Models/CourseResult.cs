using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Models
{
    public class CourseResultModel
    {
        public CourseModel SearchModel { get; set; }
        public string Topic { get; set; }
        public string CourseRegistrationNum { get; set; }

        public string Section { get; set; }

        public List<string> Location { get; set; }

        public List<string> Instructor { get; set; }

        public List<string> Time { get; set; }

        public List<string> Days { get; set; }

        public double CreditHours { get; set; }

        public List<string> ScheduleType { get; set; }

        public int SeatCap { get; set; }

        public int SeatActual { get; set; }

        public int WaitListCap {get; set;}

        public int WaitListActual { get; set; }
    }
}
