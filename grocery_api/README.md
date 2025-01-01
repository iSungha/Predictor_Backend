# Grocery API

## Introduction
This project represents the API layer of the **Grocery Predictor** application, providing functionalities to store, retrieve, and predict grocery prices using historical data and mathematical models.

---

## Getting Started

### Prerequisites
- **Visual Studio 2022 or greater**
- **Docker Desktop** for database containerization
- Install required NuGet packages when prompted.

### Setup Instructions
1. **Clone the Repository**:
   - Clone the repository and open the solution file (`grocery_api.sln`) in **Visual Studio 2022** or later.
   
2. **Install NuGet Packages**:
   - Ensure all missing NuGet packages are installed during the build process.

3. **Start Docker Desktop**:
   - Confirm Docker Desktop is installed and running. That is open the Docker Desktop app.

4. **Run the Application**:
   - Build and run the project in Visual Studio to automatically create a Docker image and start the PostgreSQL container.
5. **Run the Nuxt 3 Frontend App**:
   - Once the API is running, run the Frontend app.
   - At ('https://github.com/iSungha/Predictor_Frontend')
---

## Database Configuration

### Environment Variables
Use the following environment variables if you want to connect to the database using tools like pgAdmin or for direct SQL queries, but you don't need them for this project:
```
PORT: 5432
POSTGRES_PASSWORD: admin
POSTGRES_USER: superuser
POSTGRES_DB: GroceryDB
POSTGRES_HOST: GroceryDBHost
HOST: localhost
```

### Data Preloading
- Historical grocery price data is preloaded from a `.csv` file sourced from [Statistics Canada Grocery Data](https://www150.statcan.gc.ca/t1/tbl1/en/tv.action?pid=1810024501&pickMembers%5B0%5D=1.3&cubeTimeFrame.startMonth=06&cubeTimeFrame.startYear=2024&cubeTimeFrame.endMonth=12&cubeTimeFrame.endYear=2024&referencePeriods=20240601%2C20241201).
- **Supported Items**:
  - `Chicken breasts`
  - `Eggs`
  - `White bread`
  - `Bananas`
  - `Cereal`
  - `Yogurt`

---

## API Endpoints

### **RecordTable Endpoints**
- **Description**: Provides access to historical grocery price records.
- **Endpoints**:
  - `GET /api/RecordTable`:
    - Retrieves all preloaded historical data.
  - `GET /api/RecordTable/itemname/{productName}`:
    - Retrieves historical data for a specific product (e.g., `/api/RecordTable/itemname/Eggs`).
  - `POST /api/RecordTable/add`:
    - Allows manual addition of data beyond the preloaded dataset (e.g., data for future months).

---

### **GroceryItem Endpoints**
- **Description**: Enables prediction and retrieval of future grocery prices.
- **Endpoints**:
  - `POST /api/GroceryItem/predict/{productNameToPredict}`:
    - Predicts and stores the price for the next month based on historical data.
    - To make prediction for Eggs, you'd call POST with Eggs  (e.g., `/api/GroceryItem/predict/Eggs`).
  - `GET /api/GroceryItem/all`:
    - Retrieves all predicted prices for all items.
  - `GET /api/GroceryItem/name/{productName}`:
    - Retrieves predicted prices for a specific product (e.g., `/api/GroceryItem/name/Eggs`).

---

## Prediction Workflow

1. **Retrieve Historical Data**:
   - The API fetches historical data for the specified item from the **RecordTable**.

2. **Run Mathematical Models**:
   - Predicts the next month's price using mathematical models (e.g., EMA, Linear Regression, Polynomial Regression).

3. **Store Predictions**:
   - Predicted prices are added to the database for future reference.

4. **Retrieve Predictions**:
   - Access the predicted prices via the `GET` endpoints for individual or all items.

---

## Example Scenarios

1. **Fetch Historical Data**:
   - Endpoint: `/api/RecordTable/itemname/Eggs`
   - Response: Returns historical price data for `Eggs`.

2. **Predict Prices for Next Month**:
   - Endpoint: `POST /api/GroceryItem/predict/Eggs`
   - Action: Adds predicted price for the next month into the database.

3. **Retrieve Predicted Prices**:
   - Endpoint: `/api/GroceryItem/all` or `/api/GroceryItem/name/Eggs`
   - Response: Returns predicted prices for all items or the specified item.

---

## Troubleshooting

### Common Issues
1. **Docker Issues**:
   - Ensure Docker is running and accessible.
   - If the container fails to start, clean the solution, delete the exisitng container and image and rebuild the project in Visual Studio.

2. **NuGet Packages**:
   - Verify all necessary NuGet packages are installed during the build process.

---

## Swagger Integration

- The API integrates with Swagger for documentation and testing.
- After running the API, access Swagger at:
  ```
  https://localhost:7075/swagger/index.html
  ```
- Use the Swagger interface to test endpoints and view API documentation.

---

## Notes
- Supported item predictions are limited to preloaded historical data (`Chicken breasts`, `Eggs`, `White bread`, `Bananas`, `Cereal`, `Yogurt`).
- Predictions are calculated using three models: **EMA**, **Linear Regression**, and **Polynomial Regression**.

---

Enjoy using the **Grocery API** to forecast grocery prices and plan effectively! 🎉
