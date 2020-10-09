using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Models
{
    public class CourseResultModel : CourseModel
    {
        public string Topic { get; set; }
        public string CourseRegistrationNum { get; set; }

        public string Section { get; set; }

        public List<Meeting> Meetings { get; set; }

        public string Instructor { get; set; }

        public string CreditHours { get; set; }

        public int RemainingSeats { get; set; }

        public int RemainingWaitlist { get; set; }

        public CourseResultModel(CourseModel c)
        {
            base.SubjectCode = c.SubjectCode;
            base.CourseNumber = c.CourseNumber;
            base.CourseTitle = c.CourseTitle;
        }

    }
}
