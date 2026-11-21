# Grocery Price Prediction API

An ASP.NET Core API for querying historical grocery prices, adding price records, and storing next-month price estimates for supported products. Historical input is loaded from a bundled CSV dataset.

## Features

- Query all historical records or a product's history, and add records through the API.
- Generate next-month estimates using exponential moving average (EMA), linear regression, and polynomial regression.
- Retrieve saved estimates for all products or one product.
- Explore the API with Swagger in the Development environment.

## API overview

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/RecordTable` | List historical records |
| `GET` | `/api/RecordTable/itemname/{productName}` | Get records for one product |
| `POST` | `/api/RecordTable/add` | Add a historical price record |
| `POST` | `/api/GroceryItem/predict/{productNameToPredict}` | Generate and save the next-month estimate |
| `GET` | `/api/GroceryItem/all` | List saved estimates |
| `GET` | `/api/GroceryItem/name/{productName}` | Get estimates for one product |

## Architecture and technologies

The .NET 8 ASP.NET Core application exposes controller endpoints and registers repository interfaces through dependency injection. PostgreSQL stores historical records and estimates; Npgsql and Entity Framework Core provide database integration, while Evolve applies schema migrations. MathNet.Numerics supports the prediction calculations. The application initializes its database and loads the bundled historical CSV at startup.

## Run locally

You will need the .NET 8 SDK, Docker Desktop, and Visual Studio 2022 or later with container support. Start Docker Desktop, open [`grocery_api/grocery_api.sln`](grocery_api/grocery_api.sln) in Visual Studio, restore the NuGet packages, and run the project with its configured Docker profile. The API's Swagger UI is available under `/swagger` at the local HTTPS address shown by the project.

Keep active database credentials in secure local or deployment configuration. Do not commit secret values.
