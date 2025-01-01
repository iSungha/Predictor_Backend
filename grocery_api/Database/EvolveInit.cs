using EvolveDb;
using Npgsql;
using System.Diagnostics;

namespace grocery_api.Database
{
    public static class EvolveInit
    {
        #region Constants

        /// <summary>
        /// Connection string for the PostgreSQL database.
        /// </summary>
        private static readonly string ConnectionString = "Host=localhost;Port=5432;Database=GroceryDB;User Id=superuser;Password=admin;";

        #endregion

        #region Public Methods

        /// <summary>
        /// Synchronizes the database schema using Evolve migrations.
        /// </summary>
        public static void SyncDb()
        {
            try
            {
                Utils.Data.Connection<NpgsqlConnection>(ConnectionString, conn =>
                {
                    var evolve = new Evolve(conn, msg => Debug.WriteLine(msg))
                    {
                        Locations = new[] { "./Database/migrations" },
                        IsEraseDisabled = true, 
                    };

                    evolve.Migrate();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database migration failed: {ex}");
                throw;
            }
        }

        #endregion
    }
}
