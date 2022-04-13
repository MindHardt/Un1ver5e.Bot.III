using System;
using System.IO;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Data.SqlTypes;

namespace Un1ver5e.Bot
{
    /// <summary>
    /// Allows accessing the SQLite database for retrieving and inserting some low-level data.
    /// </summary>
    public static class Database
    {
        public static class Tokens
        {
            /// <summary>
            /// Adds a Discord Token to the Database
            /// </summary>
            /// <param name="token"></param>
            public static void AddToken(string token)
            {
                using SqliteConnection con = new("DataSource = DB.db3");
                {
                    con.Open();
                    SqliteCommand cmd = new()
                    {
                        Connection = con,
                        CommandText = $"INSERT INTO Tokens(Token) VALUES" +
                        $"('{token}')"
                    };

                    cmd.ExecuteNonQuery();
                }
            }
            /// <summary>
            /// Gets the latest Discord Token from the Database
            /// </summary>
            /// <returns></returns>
            public static string GetToken()
            {
                using SqliteConnection con = new("DataSource = DB.db3");
                {
                    con.Open();
                    SqliteCommand cmd = new()
                    {
                        Connection = con,
                        CommandText =
                        "SELECT Token " +
                        "FROM TOKENS " +
                        "ORDER BY ID DESC " +
                        "LIMIT 1"
                    };

                    var result = cmd.ExecuteScalar();

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                    return result as string;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                }
            }
        }

        public static class Feeds
        {
            /// <summary>
            /// Adds this channelID to the list of Feed Channels.
            /// </summary>
            /// <param name="channelID">The Channel ID</param>
            /// <param name="guildID">The Guild ID</param>
            public static void AddFeedChannel(ulong channelID)
            {
                using SqliteConnection con = new("DataSource = DB.db3");
                con.Open();
                SqliteCommand cmd = new()
                {
                    Connection = con,
                    CommandText = $"INSERT INTO Feeds(ChannelID) VALUES" +
                    $"('{channelID}')"
                };

                cmd.ExecuteNonQuery();
            }
            /// <summary>
            /// Adds this channelID to the list of Feed Channels.
            /// </summary>
            /// <param name="channelID">The Channel ID</param>
            /// <param name="guildID">The Guild ID</param>
            public static void RemoveFeedChannel(ulong channelID)
            {
                using SqliteConnection con = new("DataSource = DB.db3");
                {
                    con.Open();
                    SqliteCommand cmd = new()
                    {
                        Connection = con,
                        CommandText = $"REMOVE FROM Feeds " +
                        $"WHERE ChannelID = {channelID}"
                    };

                    cmd.ExecuteNonQuery();
                }
            }
            /// <summary>
            /// Gets all feed channel IDs
            /// </summary>
            /// <returns></returns>
            public static IEnumerable<ulong> GetFeedChannels()
            {
                using SqliteConnection con = new("DataSource = DB.db3");
                {
                    con.Open();
                    SqliteCommand cmd = new()
                    {
                        Connection = con,
                        CommandText =
                        "SELECT * " +
                        "FROM Feeds "
                    };

                    var result = cmd.ExecuteReader();

                    List<ulong> ids = new List<ulong>();
                    while (result.Read())
                    {
                        ids.Add((ulong)result.GetInt64("ChannelID"));
                    }
                    return ids;
                }
            }
        }

        /// <summary>
        /// Gets random minecraft splash.
        /// </summary>
        /// <returns></returns>
        public static string GetSplash()
        {
            using SqliteConnection con = new("DataSource = DB.db3");
            {
                con.Open();
                SqliteCommand cmd = new()
                {
                    Connection = con,
                    CommandText =
                    "SELECT Splash " +
                    "FROM Splashes " +
                    "ORDER BY RANDOM() " +
                    "LIMIT 1"
                };

                var result = cmd.ExecuteScalar();

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                return result as string;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            }
        }





        /// <summary>
        /// Executes given SQL query against the database.
        /// </summary>
        /// <param name="query">The SQL query to perform</param>
        /// <returns>Number of rows affected or -1 for SELECT statements.</returns>
        public static string ExecuteSqlNonQuery(string query)
        {
            using SqliteConnection con = new("DataSource = DB.db3");
            {
                con.Open();
                SqliteCommand cmd = new()
                {
                    Connection = con,
                    CommandText = query
                };

                try
                {
                    return cmd.ExecuteNonQuery().ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Executes given SQL query against the database.
        /// </summary>
        /// <param name="query">The SQL query to perform</param>
        /// <returns>The result of a query as a string.</returns>
        public static string ExecuteSqlScalar(string query)
        {
            using SqliteConnection con = new("DataSource = DB.db3");
            {
                con.Open();
                SqliteCommand cmd = new()
                {
                    Connection = con,
                    CommandText = query
                };

                try
                {
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                    return (cmd.ExecuteScalar() ?? "NOTHING FOUND.").ToString();
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        /// <summary>
        /// Returns a file of the database. Don't let it get leaked!
        /// </summary>
        /// <returns></returns>
        public static FileStream GetDatabaseBackup()
        {
            SqliteConnection.ClearAllPools();
            return File.OpenRead(Extensions.AppFolderPath + "DB.db3");
        }

        /// <summary>
        /// Deletes all current tables and creates new ones.
        /// </summary>
        public static void RestoreDatabase()
        {
            Serilog.Log.Logger.Warning("Database drop and restoration initiated!");
            using SqliteConnection con = new("DataSource = DB.db3");
            {
                con.Open();
                
                foreach (string commandText in _createTableCommands)
                {
                    SqliteCommand command = new SqliteCommand()
                    {
                        Connection = con,
                        CommandText = commandText
                    };
                    command.ExecuteNonQuery();
                }
            }
            Serilog.Log.Logger.Warning("Database dropped and restored!");
        }

        /// <summary>
        /// Gets all the table's DROP and CREATE commands.
        /// </summary>
        private static string[] _createTableCommands => new string[]
        {
            //SPLASHES
            "DROP TABLE IF EXISTS [Splashes];" +
            "CREATE TABLE [Splashes] " +
            "(" +
            "[ID] INTEGER PRIMARY KEY AUTOINCREMENT," +
            "[Splash] TEXT NOT NULL" +
            ");" + 
            "INSERT INTO [Splashes](Splash) VALUES('MO is here!')",

            //TOKENS
            "DROP TABLE IF EXISTS [Tokens];" +
            "CREATE TABLE [Tokens] " +
            "(" +
            "[ID] INTEGER PRIMARY KEY AUTOINCREMENT," +
            "[Token] TEXT NOT NULL" +
            ");" +
            "INSERT INTO [Tokens](Token) VALUES('Sample Token')",

            //FEEDS
            "DROP TABLE IF EXISTS [Feeds];" +
            "CREATE TABLE [Feeds] " +
            "(" +
            "[ID] INTEGER PRIMARY KEY AUTOINCREMENT," +
            "[ChannelID] UNSIGNED BIG INTEGER NOT NULL UNIQUE" +
            ");",
        };

        /// <summary>
        /// Formats DateTime according to ISO-8601 standart.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string AsISO8601(this DateTime datetime) => datetime.ToString("O");

        
    }
}
