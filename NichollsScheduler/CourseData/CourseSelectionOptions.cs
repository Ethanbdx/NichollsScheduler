using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Data
{
    public interface ICourseSelection
    {
        IEnumerable<CourseModel> GetCourseSelections(string Subject);
    }
    public class CourseSelectionOptions : ICourseSelection
    {
    
        public IEnumerable<CourseModel> GetCourseSelections(string Subject)
        {
            
            List<CourseModel> CourseOptions = new List<CourseModel>();
            using (StreamReader sr = new StreamReader("wwwroot/Courses.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 7)
                    {
                        CourseOptions.Add(new CourseModel
                        {
                            subject = line.Substring(0, 4),
                            courseNumber = line.Substring(5, 3)
                        });
                    }
                    else
                    {
                        CourseOptions.Add(new CourseModel
                        {
                            subject = line.Substring(0, 3),
                            courseNumber = line.Substring(4, 3)
                        });
                    }
                }
                return CourseOptions.Where(c => c.subject == Subject);
            }
        }
    }
}
