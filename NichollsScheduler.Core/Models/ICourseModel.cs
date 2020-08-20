using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Models
{
    public interface ICourseModel
    {
        string Subject { get; set; }
        string CourseNumber { get; set; }
    }
}
