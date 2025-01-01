/**************************************************************************
 * Project Name: Grocery API
 * File Name: IGroceryItemRepository.cs
 * Description: Defines the interface for grocery item repository operations, 
 * including methods for adding, retrieving by ID, and retrieving
 * all grocery items.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-24
 * Version: 1.0.0
 *************************************************************************/

using grocery_api.Models;

namespace grocery_api.Models.Repositories.Interfaces
{
    public interface IGroceryItemRepository
    {
        bool Add(GroceryItem item);
        GroceryItem GetById(Guid id);
        IEnumerable<GroceryItem> GetAll();
        bool GetPredictionByName(string productName);
        IEnumerable<GroceryItem> GetGroceryItemByName(string productName);



    }
}
