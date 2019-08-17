using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSUSchedulerLibrary
{
    class Class1
    {
        WebScraper webScraper = new WebScraper();
        public bool logIn { get; set; }
        public void attemptLogin()
        {
            Console.WriteLine("User ID");
            string user = Console.ReadLine();
            Console.WriteLine("PIN");
            string pin = Console.ReadLine();
            webScraper.loginToBanner(user, pin);
            logIn = webScraper.loggedIn;
        }
       public void getTerms()
        {
           var terms = webScraper.getAvailableTerms();
           foreach (string t in terms)
            {
                Console.WriteLine($"{terms.IndexOf(t)} : {t}");
            }
        }
        public void selectTerm()
        {
            Console.WriteLine("Select which term you'd like.");
            var selection = Int32.Parse(Console.ReadLine());
            webScraper.selectTerm(selection);
        }
        public void getRegisteredCourses()
        {
            var registeredCourses = webScraper.getRegisteredCourses();
            Console.WriteLine("You are currently registered in the following courses:" + Environment.NewLine);
            foreach(CourseModel course in registeredCourses)
            {
                Console.WriteLine($"{course.subject} {course.courseNumber} - {course.courseTitle}" + Environment.NewLine + $" Status: {course.status}");
            }
        }
        public void searchForCourse()
        {
            Console.WriteLine("Subject");
            string subject = Console.ReadLine();
            Console.WriteLine("Course Number");
            string courseNumber = Console.ReadLine();
            Console.WriteLine("Preferred Section");
            string preferredSection = Console.ReadLine();
            var results = webScraper.searchForCourse(subject, courseNumber, preferredSection);
            Console.WriteLine(Environment.NewLine + "Results:" + Environment.NewLine);
            foreach(CourseModel course in results)
            {
                Console.WriteLine($"{course.subject} {course.courseNumber} - {course.courseTitle} - {course.section}");
                Console.WriteLine($"{course.location} {course.time}");
                Console.WriteLine($"{course.instructor} {course.dateRange}");
                Console.WriteLine($"There are {course.remainingCapacity} spots available in this course.");
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
