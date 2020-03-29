using NichollsScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Data
{
    public class CourseModel : ICourseModel
    {
        public string subject { get; set; }
        public string courseNumber { get; set; }
        public string fullSubject { get; set; }
    }
}
