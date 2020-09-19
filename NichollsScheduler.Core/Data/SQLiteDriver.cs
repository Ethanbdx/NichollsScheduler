using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Data
{
    public class SQLiteDriver
    {
        public async Task<List<string>> GetCourseNumbers(string subject) {

            using var connection = new SQLiteConnection("Data Source=courses.db");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT CourseNumber FROM Courses WHERE Subject = $subject";
            command.Parameters.AddWithValue("$subject", subject);

            using var resultReader = command.ExecuteReader();
            var courseNumbers = new List<string>();
            while(resultReader.Read()) {
                var peepee = resultReader.GetString(0);
                courseNumbers.Add(resultReader.GetString(0));
                System.Console.WriteLine(peepee);
            }
            await connection.CloseAsync();
            return courseNumbers;
        }

    }
}