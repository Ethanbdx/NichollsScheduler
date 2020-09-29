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

        public Dictionary<char, List<Meeting>> Meetings { get; set; }

        public string Instructor { get; set; }

        public string CreditHours { get; set; }

        public int RemainingSeats { get; set; }

        public int RemainingWaitlist { get; set; }

    }
}
