namespace NichollsScheduler.Core.Models
{
    public class Meeting
    {
        public Time StartTime { get; set; }
        public Time EndTime { get; set;}
        public string Location { get; set; }
        public char[] Days { get; set; }
    }
}