/**************************************************************************
 * Project Name: Grocery API
 * File Name: LoadTableRepository.cs
 * Description: Repository class implementing database operations for 
 * product_prices using Npgsql and PostgreSQL. Automatically loads data 
 * from a CSV file during API startup and parses filtered product data.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-29
 * Version: 2.3.0
 *************************************************************************/

using grocery_api.Models;
using grocery_api.Models.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace grocery_api.Models.Repositories
{
    public class RecordTableRepository : IRecordTableRepository
    {
        private readonly string _connectionString;

        public RecordTableRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        #region Static SQL Class

        private static class SQL
        {
            public const string InsertData = @"
                INSERT INTO product_prices (product_name, month, price)
                VALUES (@ProductName, @Month, @Price);";

            public const string SelectAll = @"
                SELECT product_name, month, price 
                FROM product_prices;";

            public const string SelectByItemName = @"
                SELECT product_name, month, price 
                FROM product_prices
                WHERE product_name = @ProductName;";
        }

        #endregion

        #region ILoadTableRepository Implementation

        public IEnumerable<ProductPriceEntry> GetAll()
        {
            return FetchAllEntries();
        }

        public IEnumerable<ProductPriceEntry> GetByItemName(string productName)
        {
            return FetchEntriesByItemName(productName);
        }
        public bool AddProductPriceEntry(ProductPriceEntry entry)
        {
            return AddRecordToTable(_connectionString, entry);
        }

        #endregion

        #region Static Method

        /// <summary>
        /// Loads data from a CSV file into the database.
        /// This is a static method so it can be called independently.
        /// </summary>
        public static void LoadDataFromCsv(string connectionString, string filePath = "./Database/migrations/1810024501-eng.csv")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"CSV file not found at path: {filePath}");
                return;
            }

            var entries = ParseCsv(filePath);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                foreach (var (ProductName, Month, Price) in entries)
                {
                    using (var command = new NpgsqlCommand(SQL.InsertData, connection))
                    {
                        command.Parameters.AddWithValue("ProductName", NpgsqlDbType.Varchar, ProductName);
                        command.Parameters.AddWithValue("Month", NpgsqlDbType.Date, Month);
                        command.Parameters.AddWithValue("Price", NpgsqlDbType.Numeric, Price);
                        command.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("Filtered data loaded successfully into product_prices table from CSV.");
        }

        public static bool AddRecordToTable(string connectionString, ProductPriceEntry entry)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.InsertData, connection))
                {
                    command.Parameters.AddWithValue("ProductName", NpgsqlDbType.Varchar, entry.ProductName);
                    command.Parameters.AddWithValue("Month", NpgsqlDbType.Date, entry.Month);
                    command.Parameters.AddWithValue("Price", NpgsqlDbType.Numeric, entry.Price);

                    var rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Non-Static Methods

        private IEnumerable<ProductPriceEntry> FetchAllEntries()
        {
            var entries = new List<ProductPriceEntry>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.SelectAll, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add(new ProductPriceEntry
                        {
                            ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                            Month = reader.GetDateTime(reader.GetOrdinal("month")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price"))
                        });
                    }
                }
            }

            return entries;
        }

        private IEnumerable<ProductPriceEntry> FetchEntriesByItemName(string productName)
        {
            var entries = new List<ProductPriceEntry>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.SelectByItemName, connection))
                {
                    command.Parameters.AddWithValue("ProductName", NpgsqlDbType.Varchar, productName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entries.Add(new ProductPriceEntry
                            {
                                ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                                Month = reader.GetDateTime(reader.GetOrdinal("month")),
                                Price = reader.GetDecimal(reader.GetOrdinal("price"))
                            });
                        }
                    }
                }
            }

            return entries;
        }

        #endregion

        #region Helper Methods

        private static IEnumerable<(string ProductName, DateTime Month, decimal Price)> ParseCsv(string filePath)
        {
            var lines = File.ReadLines(filePath).SkipWhile(line => !line.StartsWith("\"Products\"")).ToList();
            if (lines.Count < 2)
            {
                yield break;
            }

            var relevantProducts = new HashSet<string>
    {
        "Chicken breasts", "Eggs","White bread", "Bananas", "Cereal", "Yogurt"
    };

            var headers = lines[0].Split(',').Select(col => col.Trim('"')).ToArray();
            var months = new List<(int Index, DateTime Month)>();

            for (int i = 1; i < headers.Length; i++)
            {
                if (DateTime.TryParseExact(headers[i], "MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var month))
                {
                    // Quick fix: subtract one month
                    month = month.AddMonths(-1);
                    months.Add((i, month));
                }
            }

            foreach (var line in lines.Skip(2))
            {
                var columns = line.Split(',');
                var productName = columns[0].Trim('"');

                if (string.IsNullOrWhiteSpace(productName) || !relevantProducts.Any(productName.Contains))
                {
                    continue;
                }

                foreach (var (index, month) in months)
                {
                    if (index < columns.Length && decimal.TryParse(columns[index].Trim('"'), out var price))
                    {
                        yield return (productName, month, price);
                    }
                }
            }
        }

        #endregion
    }
}
