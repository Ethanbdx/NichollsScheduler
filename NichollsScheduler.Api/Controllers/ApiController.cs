using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NichollsScheduler.Core.Business;

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
    }
}