using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NichollsScheduler.Core.Business;
using NichollsScheduler.Core.Models;

namespace NichollsScheduler.Api.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private BannerService BannerService { get; set; }

        public ApiController(BannerService bannerService) 
        {
            this.BannerService = bannerService;
        }

        [HttpGet]
        [Route("get-available-terms")]
        public async Task<IActionResult> GetAvailableTerms() 
        {
            var availableTerms = await BannerService.GetTerms();
            return Ok(availableTerms);
        }

        [HttpGet]
        [Route("get-course-subjects")]
        public async Task<IActionResult> GetCourseSubjects() {

            var courseSubjects = await BannerService.GetCourseSubjects();
            return Ok(courseSubjects);
        }

        [HttpGet]
        [Route("get-courses-info")]
        public async Task<IActionResult> GetCoursesInfo(string subject) {

            var coursesInfo = await BannerService.GetCoursesInfo(subject);
            return Ok(coursesInfo);
        }
        [HttpPost]
        [Route("search-courses")]
        public IActionResult SearchCourses([FromBody]List<CourseModel> courses, string termId) {
            var results = this.BannerService.GetCourseResults(courses, termId);
            return Ok(results);
        }
    }
}