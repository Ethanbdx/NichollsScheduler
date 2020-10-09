using System.Collections.Generic;

namespace NichollsScheduler.Core.Models
{
    public class CourseResultList
    {
        public CourseModel SearchModel { get; set; }
        public List<CourseResultModel> Results { get; set; } = new List<CourseResultModel>();

        public CourseResultList(CourseModel searchModel)
        {
            this.SearchModel = searchModel;
        }
    }
}