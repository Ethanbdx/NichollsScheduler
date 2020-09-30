using System;
namespace NichollsScheduler.Core.Models
{
    //Expressed in minutes from 0:00 AM 
    public class Time
    {
        public Time(int value, string tweleveHourTime)
        {    
            this.Value = value;
            this.TwelveHourTime = tweleveHourTime;
        }
        public int Value { get; set; }
        public string TwelveHourTime { get; set; }
        public static Tuple<Time, Time> ParseTimes(string times) {
            var timesArray = times.Split(" - ");
            var startTimeString = timesArray[0].Split(':');
            var startTimeHour = int.Parse(startTimeString[0]);
            var startTimeMinutes = int.Parse(startTimeString[1].Substring(0,2));

            var endTimeString = timesArray[1].Split(':');
            var endTimeHour = int.Parse(endTimeString[0]);
            var endTimeMinutes = int.Parse(endTimeString[1].Substring(0,2));

            var startTime = new Time(startTimeHour * 60 + startTimeMinutes, timesArray[0]);
            var endTime = new Time(endTimeHour * 60 + endTimeMinutes, timesArray[1]);

            if(timesArray[0].Contains("pm")) {
                if(startTime.Value / 60 != 12) {
                    startTime.Value += 12 * 60;
                }
                endTime.Value += 12 * 60;
            }
            
            return new Tuple<Time, Time>(startTime, endTime);
        }
    }
}