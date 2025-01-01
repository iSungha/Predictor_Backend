using System;
using System.Collections.Generic;
using System.Linq;

namespace grocery_api.Models.Math_Models
{
    public class PricePrediction
    {
        #region Exponential Moving Average (EMA)
        /// <summary>
        /// Predicts the next value using Exponential Moving Average (EMA).
        /// </summary>
        /// <param name="prices">Historical prices.</param>
        /// <param name="smoothingFactor">Smoothing factor for EMA calculation.</param>
        /// <returns>Predicted price for the next period.</returns>
        public static double PredictWithEMA(List<double> prices, int smoothingFactor = 2)
        {
            if (prices == null || prices.Count == 0)
                throw new ArgumentException("Prices list cannot be null or empty.");

            double ema = prices[0];
            double multiplier = smoothingFactor / (double)(prices.Count + 1);

            foreach (var price in prices.Skip(1))
            {
                ema = (price - ema) * multiplier + ema;
            }

            return ema;
        }
        #endregion

        #region Linear Regression
        /// <summary>
        /// Predicts the next value using Linear Regression.
        /// </summary>
        /// <param name="months">List of DateTime representing historical periods.</param>
        /// <param name="prices">Historical prices.</param>
        /// <returns>Predicted price for the next period.</returns>
        public static double PredictWithLinearRegression(List<DateTime> months, List<double> prices)
        {
            if (months == null || prices == null || months.Count != prices.Count || months.Count < 2)
                throw new ArgumentException("Insufficient or mismatched data for linear regression.");

            int n = prices.Count;
            double sumX = months.Select((date, index) => (double)index).Sum();
            double sumY = prices.Sum();
            double sumXY = months.Select((date, index) => index * prices[index]).Sum();
            double sumXX = months.Select((date, index) => (double)(index * index)).Sum();

            double denominator = (n * sumXX - sumX * sumX);
            if (Math.Abs(denominator) < 1e-10)
                return prices.Last();

            double slope = (n * sumXY - sumX * sumY) / denominator;
            double intercept = (sumY - slope * sumX) / n;

            return slope * n + intercept;
        }
        #endregion

        #region Polynomial Regression
        /// <summary>
        /// Predicts the next value using Polynomial Regression.
        /// </summary>
        /// <param name="months">List of DateTime representing historical periods.</param>
        /// <param name="prices">Historical prices.</param>
        /// <param name="degree">Degree of the polynomial.</param>
        /// <returns>Predicted price for the next period.</returns>
        public static double PredictWithPolynomialRegression(List<DateTime> months, List<double> prices, int degree = 2)
        {
            if (months == null || prices == null || months.Count != prices.Count || months.Count < 2)
                throw new ArgumentException("Insufficient or mismatched data for polynomial regression.");

            // Normalize the indices
            var xData = months.Select((date, index) => (double)index / (months.Count - 1)).ToArray();
            var yData = prices.ToArray();

            // Restrict degree to avoid overfitting
            degree = Math.Min(degree, prices.Count - 1);

            // Log data for debugging
            Console.WriteLine("xData: " + string.Join(", ", xData));
            Console.WriteLine("yData: " + string.Join(", ", yData));
            Console.WriteLine("Using polynomial degree: " + degree);

            try
            {
                var polyCoeffs = MathNet.Numerics.Fit.Polynomial(xData, yData, degree);
                Console.WriteLine("Polynomial Coefficients: " + string.Join(", ", polyCoeffs));

                double nextIndex = 1.0 + 1.0 / (months.Count - 1);
                double prediction = polyCoeffs.Reverse().Select((c, i) => c * Math.Pow(nextIndex, i)).Sum();

                double maxPrice = prices.Max();
                double minPrice = prices.Min();

                if (prediction < minPrice * 0.5 || prediction > maxPrice * 2.0)
                {
                    Console.WriteLine("Warning: Polynomial regression produced an unrealistic value. Returning last known price.");
                    return prices.Last();
                }

                return prediction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Polynomial Regression: {ex.Message}");
                return prices.Last();
            }
        }
        #endregion
    }
}
