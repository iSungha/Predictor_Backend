/**************************************************************************
 * Project Name: Grocery API
 * File Name: ProductPriceEntry.cs
 * Description: Model class representing an entry in the product_prices table.
 * Includes properties for product name, month, and price.
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
    /// Represents an entry in the product_prices table.
    /// </summary>
    public class ProductPriceEntry
    {
        #region Properties

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The month associated with the price.
        /// </summary>
        public DateTime Month { get; set; }

        /// <summary>
        /// The price of the product.
        /// </summary>
        public decimal Price { get; set; }

        #endregion
    }
    #endregion
}
