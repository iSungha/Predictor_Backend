using System;
using System.Diagnostics;
using System.Threading;
using Npgsql;

namespace grocery_api.Database
{
    public static class DatabaseStartup
    {
        #region Public Methods

        /// <summary>
        /// Runs the database startup script and waits for the database to become ready.
        /// </summary>
        public static void RunDatabaseStartupScript()
        {
            try
            {
                StartDatabaseScript();
                WaitForDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while running database startup script: {ex.Message}");
            }
        }

        #endregion

        #region Private DB Starter Methods

        /// <summary>
        /// Starts the database startup script.
        /// </summary>
        private static void StartDatabaseScript()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c \"local_postgres_database_start.bat\"",
                        WorkingDirectory = "./Database",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.Start();

                // Capture output and errors
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Error running database startup script: {error}");
                }
                else
                {
                    Console.WriteLine($"Database startup script output: {output}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while starting database script: {ex.Message}");
            }
        }

        /// <summary>
        /// Waits for the database to become ready by repeatedly attempting a connection.
        /// </summary>
        private static void WaitForDatabase()
        {
            string connectionString = "Host=localhost;Port=5432;Database=GroceryDB;Username=superuser;Password=admin";

            const int maxRetries = 10;
            const int delayMs = 3000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        Console.WriteLine("Database is ready.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt}: Database not ready yet. Retrying in {delayMs / 1000} seconds...");
                    Console.WriteLine($"Error: {ex.Message}");
                    Thread.Sleep(delayMs);
                }
            }

            Console.WriteLine("Database did not become ready in time. Please check the container logs.");
            throw new Exception("Database startup timeout.");
        }

        #endregion
    }
}
