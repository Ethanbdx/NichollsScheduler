using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Data
{
    public static class SQLiteDriver
    {
        public static string DB_PATH = @"Data Source=E:\source\repos\NichollsScheduler\courses.db";
        public static async Task<List<string>> GetCourseNumbers(string subject) {

            using var connection = new SQLiteConnection(DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT CourseNumber FROM Courses WHERE Subject = $subject";
            command.Parameters.AddWithValue("$subject", subject);

            using var resultReader = await command.ExecuteReaderAsync();
            var courseNumbers = new List<string>();
            while(resultReader.Read()) {
                courseNumbers.Add(resultReader.GetString(0));
            }
            await connection.CloseAsync();
            return courseNumbers;
        }

        public static async Task<List<string>> GetCourseSubjects() {
            using var connection = new SQLiteConnection(DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT Subject FROM Courses";

            using var resultReader = await command.ExecuteReaderAsync();
            var courseSubjects = new List<string>();
            while (resultReader.Read()) {
                courseSubjects.Add(resultReader.GetString(0));
            }
            await connection.CloseAsync();
            return courseSubjects;
        }
    }
}