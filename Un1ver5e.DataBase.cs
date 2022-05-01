using Npgsql;
using System.Data;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// Contains methods used to access Database.
    /// </summary>
    public static class PSQLDatabase
    {
        private readonly static string _connString =
            "Host=balarama.db.elephantsql.com;" +
            "Username=pcnyawpn;" +
            "Password=eNuTlM2eCJrG-qTnYw2hfNhrzIGJYczx;" +
            "Database=pcnyawpn";

        private static NpgsqlConnection GetOpenedConnection()
        {
            var con = new NpgsqlConnection(_connString);
            con.Open();
            return con;
        }

        public static string GetSplash()
        {
            using (NpgsqlConnection con = GetOpenedConnection())
            {
                NpgsqlCommand command = new()
                {
                    Connection = con,
                    CommandText = $"SELECT splash FROM splashes ORDER BY RANDOM() LIMIT 1"
                };
                return (string)command.ExecuteScalar()!;
            }
        }
    }
}
