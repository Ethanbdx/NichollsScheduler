using System;
namespace NichollsScheduler.Core.Models
{
    public class Time
    {
        public Time(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;    
        }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public string TwelveHourTime {
            get {
                if(this.Hour > 12) {
                    return string.Format("{0}:{1:00} pm", this.Hour - 12, this.Minute);
                }
                else if (this.Hour < 12) {
                    return string.Format("{0}:{1:00} am", this.Hour, this.Minute);
                } 
                else {
                    return string.Format("{0}:{1:00} pm", this.Hour, this.Minute);
                }
            }
        }
        public static Tuple<Time, Time> ParseTimes(string times) {
            var timesArray = times.Split(" - ");
            var startTimeString = timesArray[0].Split(':');
            var startTimeHour = int.Parse(startTimeString[0]);
            var startTimeMinutes = int.Parse(startTimeString[1].Substring(0,2));

            var endTimeString = timesArray[1].Split(':');
            var endTimeHour = int.Parse(endTimeString[0]);
            var endTimeMinutes = int.Parse(endTimeString[1].Substring(0,2));

            var startTime = new Time(startTimeHour, startTimeMinutes);
            var endTime = new Time(endTimeHour, endTimeMinutes);

            if(timesArray[0].Contains("pm")) {
                if(startTime.Hour != 12) {
                    startTime.Hour += 12;
                }
                endTime.Hour += 12;
            }
            
            return new Tuple<Time, Time>(startTime, endTime);
        }
    }
}