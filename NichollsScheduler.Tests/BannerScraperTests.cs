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
            var bannerScraper = new BannerScraper();
            var terms = await bannerScraper.GetTerms();
        }
        [TestMethod]
        public void GetCourseResults() {
            var bannerScraper = new BannerScraper();
            var courseQuery = new List<CourseModel> {
                new CourseModel {
                    Subject = "ENGL",
                    CourseNumber = "101"
                },
                new CourseModel {
                    Subject = "SPCH",
                    CourseNumber = "101"
                },
                new CourseModel {
                    Subject = "MATH",
                    CourseNumber = "101"
                }
            };
            var courseResults = bannerScraper.GetCourseResults(courseQuery, "202080");
        }
    }
}
