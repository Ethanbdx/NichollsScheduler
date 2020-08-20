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
                    Subject = "MATH",
                    CourseNumber = "101"
                },
                new CourseModel {
                    Subject = "SPCH",
                    CourseNumber = "101"
                }
            };
            var courseResults = BannerScraper.GetCourseResults(courseQuery, "202080");
            stopWatch.Stop();
            
            foreach(var course in courseResults) {
                Console.WriteLine(course.Count);
            }
            Console.WriteLine(stopWatch.Elapsed);
        }
    }
}
