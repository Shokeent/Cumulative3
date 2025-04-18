using MySql.Data.MySqlClient;


namespace Cumulative01.Models
{
    public class SchoolDbContext
    {
        //this set our credentials to connect to the database

        private static string User { get { return "root"; } }
        private static string Password { get { return ""; } }
        private static string Database { get { return "schooldb"; } }
        private static string Server { get { return "localhost"; } }
        private static string Port { get { return "3306"; } }

        // method will return the connection string to connect to the schoolDb


        protected static string ConnectionString
        {
            get
            {
                return "server=" + Server
                    + ";user=" + User
                    + ";database=" + Database
                    + ";port=" + Port
                    + ";password=" + Password
                    +"; convert zero datetime = True";
            }
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }

}
