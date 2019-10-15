using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Data
{
    public class CourseResult
    {
        public string subject { get; set; }

        public string courseNumber { get; set; }

        public string courseRegistrationNum { get; set; }

        public string section { get; set; }

        public List<string> location { get; set; }

        public List<string> instructor { get; set; }

        public List<string> time { get; set; }

        public List<string> days { get; set; }

        public string creditHours { get; set; }

        public string title { get; set; }

        public List<string> scheduleType { get; set; }

        public int seatCap { get; set; }

        public int seatActual { get; set; }

        public int waitListCap {get; set;}

        public int waitListActual { get; set; }
    }
}
