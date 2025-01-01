/**************************************************************************
 * Project Name: Grocery API
 * File Name: GroceryItemController.cs
 * Description: This controller handles API endpoints for managing grocery 
 * items. It supports operations such as adding, retrieving 
 * by ID, and retrieving all items.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-24
 * Version: 1.0.0
 *************************************************************************/

using Microsoft.AspNetCore.Mvc;
using grocery_api.Models;
using grocery_api.Models.Repositories.Interfaces;

namespace grocery_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroceryItemController : ControllerBase
    {
        private readonly IGroceryItemRepository _repository;

        #region Constructor

        public GroceryItemController(IGroceryItemRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Endpoints

        /*
         * Data to this table should only be posted by repo class after predicting next month's price, not manually.
         * POST is left here for debugging purposes only.
         *
        [HttpPost("add")]
        public IActionResult AddItem([FromBody] GroceryItem item)
        {
            try
            {
                var isAdded = _repository.Add(item);
                return isAdded ? Ok($"Item '{item.ItemName}' added successfully.") : BadRequest("Failed to add item.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        */

        /// <summary>
        /// Gets a grocery item by its ID.
        /// </summary>
        /// <param name="id">The ID of the grocery item.</param>
        /// <returns>The grocery item if found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetItemById(Guid id)
        {
            try
            {
                var item = _repository.GetById(id);
                return item != null ? Ok(item) : NotFound("Item not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all grocery items.
        /// </summary>
        /// <returns>A list of all grocery items.</returns>
        [HttpGet("all")]
        public IActionResult GetAllItems()
        {
            try
            {
                var items = _repository.GetAll();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Predicts and adds the next month's price for the specified grocery item.
        /// </summary>
        /// <param name="productName">The name of the product to predict prices for.</param>
        /// <returns>A success or failure message.</returns>
        [HttpPost("predict/{productNameToPredict}")]
        public IActionResult PredictAndAddNextMonthPrice(string productNameToPredict)
        {
            if (string.IsNullOrWhiteSpace(productNameToPredict))
            {
                return BadRequest("Product name cannot be empty.");
            }

            var success = _repository.GetPredictionByName(productNameToPredict);

            if (success)
            {
                return Ok($"Predictions for {productNameToPredict} were successfully added.");
            }

            return NotFound($"Could not predict prices for {productNameToPredict}. Ensure the product exists in historical data.");
        }

        /// <summary>
        /// Gets grocery items by their name.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <returns>A list of matching grocery items.</returns>
        [HttpGet("name/{productName}")]
        public IActionResult GetGroceryItemsByName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest("Product name cannot be empty.");
            }

            try
            {
                var items = _repository.GetGroceryItemByName(productName);
                if (!items.Any())
                {
                    return NotFound($"No records found for product name: {productName}");
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
