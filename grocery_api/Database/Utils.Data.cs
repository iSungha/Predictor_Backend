#region Using directives
using System;
using System.Data;
#endregion

namespace grocery_api.Database
{
    public static partial class Utils
    {
        public static partial class Data
        {
            #region Public Methods

            /// <summary>
            /// Provides a connection to the database and performs the given action.
            /// </summary>
            /// <typeparam name="TConn">The type of the database connection (e.g., NpgsqlConnection).</typeparam>
            /// <param name="connectionString">The connection string to establish the database connection.</param>
            /// <param name="action">The action to perform with the open connection.</param>
            public static void Connection<TConn>(string connectionString, Action<TConn> action)
                where TConn : IDbConnection, new()
            {
                using (var conn = new TConn { ConnectionString = connectionString })
                {
                    conn.Open();
                    action(conn);
                }
            }

            #endregion
        }
    }
}
