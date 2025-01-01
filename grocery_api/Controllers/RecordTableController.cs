/**************************************************************************
 * Project Name: Grocery API
 * File Name: LoadTableController.cs
 * Description: API Controller for managing load_table data, including 
 * retrieving all data.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-24
 * Version: 1.1.0
 *************************************************************************/

using grocery_api.Models;
using grocery_api.Models.Repositories;
using grocery_api.Models.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace grocery_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordTableController : ControllerBase
    {
        private readonly IRecordTableRepository _repository;

        #region Constructor

        public RecordTableController(IRecordTableRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Retrieves all entries in the load table.
        /// </summary>
        /// <returns>A list of all product price entries.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<ProductPriceEntry>> GetAll()
        {
            try
            {
                var results = _repository.GetAll();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves product price entries by product name.
        /// </summary>
        /// <param name="productName">The name of the product to retrieve data for.</param>
        /// <returns>A list of matching product price entries.</returns>
        [HttpGet("itemname/{productName}")]
        public IActionResult GetByItemName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest("Product name cannot be empty.");
            }

            try
            {
                var results = _repository.GetByItemName(productName);
                if (!results.Any())
                {
                    return NotFound($"No records found for product name: {productName}");
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a new product price entry to the table.
        /// </summary>
        /// <param name="entry">The <see cref="ProductPriceEntry"/> to add.</param>
        /// <returns>A success message if inserted; otherwise, a bad request.</returns>
        [HttpPost("add")]
        public IActionResult AddProductPriceEntry([FromBody] ProductPriceEntry entry)
        {
            if (entry == null)
                return BadRequest("Entry cannot be null");

            var success = _repository.AddProductPriceEntry(entry);
            if (success)
                return Ok("Record inserted successfully.");

            return BadRequest("Failed to insert record.");
        }
    }

    #endregion
}
