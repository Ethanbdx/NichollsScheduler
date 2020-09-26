using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var bannerScraper = new BannerScraper();
            var terms = await bannerScraper.GetTerms();
        }
        [TestMethod]
        public void GetCourseResults() {
            var bannerScraper = new BannerScraper();
            var courseQuery = new List<CourseModel> {
                new CourseModel {
                    SubjectCode = "ENGL",
                    CourseNumber = "101"
                },
                new CourseModel {
                    SubjectCode = "SPCH",
                    CourseNumber = "101"
                },
                new CourseModel {
                    SubjectCode = "MATH",
                    CourseNumber = "101"
                }
            };
            var courseResults = bannerScraper.GetCourseResults(courseQuery, "202080");
        }
    }
    [TestClass]
    public class SqliteDriverTests {
        [TestMethod]
        public async Task GetEnglishCourseNumbers() {
            await SQLiteDriver.GetCoursesInfo("ENGL");
        }
        [TestMethod]
        public async Task TaskGetMathCourseNumbers() {
            await SQLiteDriver.GetCoursesInfo("MATH");
        }
    }
}
