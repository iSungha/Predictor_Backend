/**************************************************************************
 * Project Name: Grocery API
 * File Name: GroceryItem.cs
 * Description: Represents a model for a grocery item, including properties 
 * such as Year, Month, ItemName, and Price, along with 
 * an auto-generated unique identifier.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-24
 * Version: 1.0.0
 *************************************************************************/

#region Namespace
namespace grocery_api.Models
#endregion
{
    #region Class Definition
    /// <summary>
    /// Represents a grocery item with properties for tracking its details.
    /// </summary>
    public class GroceryItem
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the grocery item.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Year associated with the grocery item's record.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month associated with the grocery item's record.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Name of the grocery item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Price of the grocery item.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The model used to predict the price of the grocery item.
        /// </summary>
        public string Model_Used { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GroceryItem"/> class.
        /// </summary>
        /// <param name="id">Unique identifier for the grocery item.</param>
        /// <param name="year">Year associated with the record.</param>
        /// <param name="month">Month associated with the record.</param>
        /// <param name="itemName">Name of the grocery item.</param>
        /// <param name="price">Price of the grocery item.</param>
        /// <param name="model_used">The model used for price prediction.</param>
        public GroceryItem(Guid id, int year, int month, string itemName, decimal price, string model_used)
        {
            Id = id;
            Year = year;
            Month = month;
            ItemName = itemName;
            Price = price;
            Model_Used = model_used;
        }

        #endregion
    }
    #endregion
}
