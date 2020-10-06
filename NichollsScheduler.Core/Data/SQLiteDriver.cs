using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Data
{
    public static class SQLiteDriver
    {
        public static string DB_PATH = $"Data Source={Directory.GetCurrentDirectory()}\\courses.db";
    }
}