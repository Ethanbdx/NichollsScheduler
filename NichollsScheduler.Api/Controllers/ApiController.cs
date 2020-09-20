using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NichollsScheduler.Core.Business;
using NichollsScheduler.Core.Data;

namespace NichollsScheduler.Api.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private BannerScraper BannerScraper { get; set; }

        public ApiController(BannerScraper bannerScraper) 
        {
            this.BannerScraper = bannerScraper;
        }

        [HttpGet]
        [Route("get-available-terms")]
        public async Task<IActionResult> GetAvailableTerms() 
        {
            var availableTerms = await BannerScraper.GetTerms();
            return Ok(availableTerms);
        }

        [HttpGet]
        [Route("get-course-subjects")]
        public async Task<IActionResult> GetCourseSubjects() {

            var courseSubjects = await SQLiteDriver.GetCourseSubjects();
            return Ok(courseSubjects);
        }

        [HttpGet]
        [Route("get-courses-info")]
        public async Task<IActionResult> GetCoursesInfo(string subject) {

            var coursesInfo = await SQLiteDriver.GetCoursesInfo(subject);
            return Ok(coursesInfo);
        }
    }
}