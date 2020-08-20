using Microsoft.VisualStudio.TestTools.UnitTesting;
using NichollsScheduler.Core.Business;
using NichollsScheduler.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NichollsScheduler.Tests {
    [TestClass]
    public class BannerScraperTests {
        [TestMethod]
        public async Task GetTerms() {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var terms = await BannerScraper.GetTerms();
            stopWatch.Stop();
            foreach(var kvp in terms) {
                Console.WriteLine($"{kvp.Key} : {kvp.Value}");
            }
            Console.WriteLine(stopWatch.Elapsed);
        }
        [TestMethod]
        public void GetCourseResults() {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var courseQuery = new List<CourseModel> {
                new CourseModel {
                    Subject = "ENGL",
                    CourseNumber = "101"
                },
                new CourseModel {
                    Subject = "ENGL",
                    CourseNumber = "215"
                }
            };
            var courseResults = BannerScraper.GetCourseResults(courseQuery, "202080");
            stopWatch.Stop();
            
            foreach(var courseList in courseResults) {
                Console.WriteLine("------------------");
                foreach(var course in courseList) {
                    Console.WriteLine($"CRN: {course.CourseRegistrationNum}");
                    Console.WriteLine($"Subject: {course.Subject}");
                    Console.WriteLine($"Course Number: {course.CourseNumber}");
                    Console.WriteLine($"Title: {course.Title}");
                    if(course.Topic != null) {
                        Console.WriteLine($"Topic: {course.Topic}");
                    }
                }
                Console.WriteLine("------------------");
            }
            Console.WriteLine(stopWatch.Elapsed);
        }
    }
}
