using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSUSchedulerLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            Class1 test = new Class1();
            while (test.logIn != true)
            {
                test.attemptLogin();
            }
            test.getTerms();
            test.selectTerm();
            test.getRegisteredCourses();
            test.searchForCourse();
        }
    }
}
