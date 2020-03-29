using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Models
{
    public interface ICourseModel
    {
        string subject { get; set; }
        string courseNumber { get; set; }
    }
}
