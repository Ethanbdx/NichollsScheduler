using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSUSchedulerLibrary
{
    class CourseModel
    {
        public string status { get; set; }
        public string CRN { get; set; }
        public string subject { get; set; }
        public string courseNumber { get; set; }
        public string section { get; set; }
        public string creditHours { get; set; }
        public string courseTitle { get; set; }
        public string time { get; set; }
        public string remainingCapacity { get; set; }
        public string instructor { get; set; }
        public string dateRange { get; set; }
        public string location { get; set; }

        public CourseModel()
        {

        }
        public CourseModel(string status, string crn, string subject, string courseNumber, string section, string creditHours, string courseTitle)
        {
            this.status = status;
            this.CRN = crn;
            this.subject = subject;
            this.courseNumber = courseNumber;
            this.subject = subject;
            this.section = section;
            this.creditHours = creditHours;
            this.courseTitle = courseTitle;
        }

        public CourseModel(string crn, string subject, string courseNumber, string section, string creditHours,  string courseTitle, string time, string remainingCapacity, string instructor, string dateRange, string location)
        {
            this.CRN = crn;
            this.subject = subject;
            this.courseNumber = courseNumber;
            this.section = section;
            this.courseTitle = courseTitle;
            this.time = time;
            this.remainingCapacity = remainingCapacity;
            this.instructor = instructor;
            this.dateRange = dateRange;
            this.location = location;
        }
    }
}
