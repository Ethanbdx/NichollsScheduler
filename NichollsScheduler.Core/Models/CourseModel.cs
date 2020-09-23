using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Models
{
    public class CourseModel : ICourseModel
    {
        public CourseModel() {

        }
        public string SubjectCode { get; set; }
        public string CourseNumber { get; set; }
        public string CourseTitle { get; set; }
    }
}
