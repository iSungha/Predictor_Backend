/**************************************************************************
 * Project Name: Grocery API
 * File Name: ILoadTableRepository.cs
 * Description: Interface for load_table repository, providing methods 
 * for retrieving all data.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-24
 * Version: 1.1.0
 *************************************************************************/

using grocery_api.Models;

namespace grocery_api.Models.Repositories.Interfaces
{
    public interface IRecordTableRepository
    {
        IEnumerable<ProductPriceEntry> GetAll();
        IEnumerable<ProductPriceEntry> GetByItemName(string pdoductName);
        bool AddProductPriceEntry(ProductPriceEntry entry);
    }
}
