using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NichollsScheduler.Core.Business;
using NichollsScheduler.Core.Data;
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
            var bannerScraper = new BannerService();
            var terms = await bannerScraper.GetTerms();
        }
        [TestMethod]
        public void GetCourseResults() {
            var bannerService = new BannerService();
            var courseQuery = new List<CourseModel> {
                new CourseModel {
                    SubjectCode = "SPCH",
                    CourseNumber = "101"
                },
                new CourseModel {
                    SubjectCode = "ACCT",
                    CourseNumber = "205"
                },
                new CourseModel {
                    SubjectCode = "BIOL",
                    CourseNumber = "155"
                },
                new CourseModel {
                    SubjectCode = "ENGL",
                    CourseNumber = "101"
                },
                new CourseModel {
                    SubjectCode = "MATH",
                    CourseNumber = "101"
                }
            };
            var sw = new Stopwatch();
            sw.Start();
            var courseResults = bannerService.GetCourseResults(courseQuery, "202080");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed);
            string json = JsonConvert.SerializeObject(courseResults);
            System.IO.File.WriteAllText(@"E:\source\repos\NichollsScheduler\NichollsScheduler.Tests\test.json", json);
        }
        [TestMethod]
        public async Task GetEnglishCourseNumbers() {
            var bannerService = new BannerService();
            await bannerService.GetCoursesInfo("ENGL");
        }
        [TestMethod]
        public async Task TaskGetMathCourseNumbers() {
            var bannerService = new BannerService();
            await bannerService.GetCoursesInfo("MATH");
        }
    }
}
