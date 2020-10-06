using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Data
{
    public static class SQLiteDriver
    {
        public static string DB_PATH = $"Data Source={Directory.GetCurrentDirectory()}\\courses.db";
        public static async Task<List<object>> GetCoursesInfo(string subject) {

            using var connection = new SQLiteConnection(DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT CourseNumber, CourseTitle FROM Courses WHERE Subject = $subject";
            command.Parameters.AddWithValue("$subject", subject);

            using var resultReader = await command.ExecuteReaderAsync();
            var coursesInfo = new List<object>();
            while(resultReader.Read()) {
                coursesInfo.Add(new { courseNumber = resultReader.GetString(0), courseTitle = resultReader.GetString(1) });
            }
            await connection.CloseAsync();
            return coursesInfo;
        }

        public static async Task<List<object>> GetCourseSubjects() {
            using var connection = new SQLiteConnection(DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT FullSubject, SubjectCode FROM Subjects";

            using var resultReader = await command.ExecuteReaderAsync();
            var courseSubjects = new List<object>();
            while (resultReader.Read()) {
                //anonymous object for subjects
                var subject = new { fullSubject = resultReader.GetString(0), subjectCode = resultReader.GetString(1) };
                courseSubjects.Add(subject);
            }
            await connection.CloseAsync();
            return courseSubjects;
        }
    }
}