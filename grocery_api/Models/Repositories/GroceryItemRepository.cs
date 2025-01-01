/**************************************************************************
 * Project Name: Grocery API
 * File Name: GroceryItemRepository.cs
 * Description: Repository class implementing database operations for 
 * grocery items using Npgsql and PostgreSQL. Provides methods 
 * for adding, retrieving by ID, and retrieving all items.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-29
 * Version: 1.1.0
 *************************************************************************/

using grocery_api.Models;
using grocery_api.Models.Math_Models;
using grocery_api.Models.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace grocery_api.Models.Repositories
{
    public class GroceryItemRepository : IGroceryItemRepository
    {
        private readonly string _connectionString;
        private readonly IRecordTableRepository _loadTableRepository;

        public GroceryItemRepository(IConfiguration configuration, IRecordTableRepository loadTableRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _loadTableRepository = loadTableRepository;

        }

        #region Static SQL Class

        private static class SQL
        {
            public const string InsertItem = @"
                INSERT INTO Prices (Year, Month, ItemName, Price, Model_Used)
                VALUES (@Year, @Month, @ItemName, @Price, @Model_Used)
                RETURNING Id;";

            public const string SelectAll = @"
                SELECT Id, Year, Month, ItemName, Price, Model_Used
                FROM Prices;";

            public const string SelectById = @"
                SELECT Id, Year, Month, ItemName, Price, Model_Used
                FROM Prices
                WHERE Id = @Id;";
            public const string SelectByName = @"
                SELECT Id, Year, Month, ItemName, Price, Model_Used
                FROM Prices
                WHERE ItemName = @ItemName;";
        }

        #endregion

        #region IGroceryItemRepository Implementation

        public bool Add(GroceryItem item) => AddItem(item);
        public GroceryItem GetById(Guid id) => GetItemById(id);
        public IEnumerable<GroceryItem> GetAll() => GetAllItems();
        public IEnumerable<GroceryItem> GetGroceryItemByName(string productName) => FetchGroceryItemsByName(productName);
        public bool GetPredictionByName(string productName)
        {
            return PricePredictionHelper.PredictAndAddNextMonthPrice(
                productName,
                _loadTableRepository,
                Add);
        }



        #endregion

        #region Non-Static Methods

        private bool AddItem(GroceryItem item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.InsertItem, connection))
                {
                    InsertParameters(command, item);

                    var generatedId = command.ExecuteScalar();
                    if (generatedId != null)
                    {
                        item.GetType()
                            .GetProperty(nameof(GroceryItem.Id))?
                            .SetValue(item, (Guid)generatedId);
                        return true;
                    }

                    return false;
                }
            }
        }

        private GroceryItem GetItemById(Guid id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.SelectById, connection))
                {
                    command.Parameters.AddWithValue("Id", NpgsqlDbType.Uuid, id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToGroceryItem(reader);
                        }
                    }
                }
            }

            return null!;
        }

        private IEnumerable<GroceryItem> GetAllItems()
        {
            var items = new List<GroceryItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.SelectAll, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapReaderToGroceryItem(reader));
                    }
                }
            }

            return items;
        }

        private IEnumerable<GroceryItem> FetchGroceryItemsByName(string productName)
        {
            var items = new List<GroceryItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(SQL.SelectByName, connection))
                {
                    command.Parameters.AddWithValue("ItemName", NpgsqlDbType.Text, productName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(MapReaderToGroceryItem(reader));
                        }
                    }
                }
            }

            return items;
        }


        #endregion

        #region Helper Methods

        private GroceryItem MapReaderToGroceryItem(NpgsqlDataReader reader)
        {
            return new GroceryItem(
                reader.GetGuid(reader.GetOrdinal("Id")),
                reader.GetInt32(reader.GetOrdinal("Year")),
                reader.GetInt32(reader.GetOrdinal("Month")),
                reader.GetString(reader.GetOrdinal("ItemName")),
                reader.GetDecimal(reader.GetOrdinal("Price")),
                reader.GetString(reader.GetOrdinal("Model_Used"))
            );
        }

        private void InsertParameters(NpgsqlCommand command, GroceryItem item)
        {
            command.Parameters.AddWithValue("Year", NpgsqlDbType.Integer, item.Year);
            command.Parameters.AddWithValue("Month", NpgsqlDbType.Integer, item.Month);
            command.Parameters.AddWithValue("ItemName", NpgsqlDbType.Text, item.ItemName);
            command.Parameters.AddWithValue("Price", NpgsqlDbType.Numeric, item.Price);
            command.Parameters.AddWithValue("Model_Used", NpgsqlDbType.Text, item.Model_Used);
        }

        #endregion
    }
}
