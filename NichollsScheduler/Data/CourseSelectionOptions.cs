using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Data
{
    public interface ICourseSelection
    {
        IEnumerable<Course> GetCourseSelections(string Subject);
    }
    public class CourseSelectionOptions : ICourseSelection
    {
    
        public IEnumerable<Course> GetCourseSelections(string Subject)
        {
            List<Course> CourseOptions = new List<Course>();
            using (StreamReader sr = new StreamReader("Courses.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 7)
                    {
                        CourseOptions.Add(new Course
                        {
                            subject = line.Substring(0, 4),
                            courseNum = line.Substring(5, 3)
                        });
                    }
                    else
                    {
                        CourseOptions.Add(new Course
                        {
                            subject = line.Substring(0, 3),
                            courseNum = line.Substring(4, 3)
                        });
                    }
                }
                return CourseOptions.Where(c => c.subject == Subject);
            }
        }
    }
}
