using System;
using System.Collections.Generic;
using System.Linq;
using grocery_api.Models.Math_Models;
using grocery_api.Models;
using grocery_api.Models.Repositories.Interfaces;

namespace grocery_api.Models.Math_Models
{
    public static class PricePredictionHelper
    {
        /// <summary>
        /// Predicts the next month's price for a given product using various models and adds the predictions to the database.
        /// </summary>
        /// <param name="productName">Name of the product.</param>
        /// <param name="loadTableRepository">Repository for fetching historical data.</param>
        /// <param name="addToDatabase">Delegate to add the GroceryItem to the database.</param>
        /// <returns>True if predictions were successfully added; otherwise, false.</returns>
        public static bool PredictAndAddNextMonthPrice(
            string productName,
            IRecordTableRepository loadTableRepository,
            Func<GroceryItem, bool> addToDatabase)
        {
            // Step 1: Retrieve historical data
            var historicalData = loadTableRepository.GetByItemName(productName)
                .OrderBy(entry => entry.Month)
                .ToList();

            if (!historicalData.Any())
            {
                Console.WriteLine($"No historical data found for {productName}.");
                return false;
            }

            var prices = historicalData.Select(entry => (double)entry.Price).ToList();
            var months = historicalData.Select(entry => entry.Month).ToList();

            var lastMonth = months.Last();
            var nextMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1).AddMonths(1);

            var predictions = new List<(double PredictedPrice, string ModelUsed)>();

            try
            {
                predictions.Add((PricePrediction.PredictWithEMA(prices), "EMA"));
                predictions.Add((PricePrediction.PredictWithLinearRegression(months, prices), "Linear Regression"));
                predictions.Add((PricePrediction.PredictWithPolynomialRegression(months, prices), "Polynomial Regression"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error predicting price for {productName}: {ex.Message}");
                return false;
            }

            foreach (var (predictedPrice, modelUsed) in predictions)
            {
                if (predictedPrice <= 0)
                {
                    Console.WriteLine($"Invalid predicted price for {productName} using {modelUsed}: {predictedPrice}");
                    continue;
                }

                var newItem = new GroceryItem(
                    Guid.NewGuid(),
                    nextMonth.Year,
                    nextMonth.Month,
                    productName,
                    (decimal)predictedPrice,
                    modelUsed
                );

                if (!addToDatabase(newItem))
                {
                    Console.WriteLine($"Failed to add prediction for {productName} using {modelUsed}.");
                    return false;
                }

                Console.WriteLine($"Successfully added prediction for {productName} using {modelUsed}: {predictedPrice}");
            }

            return true;
        }
    }
}
