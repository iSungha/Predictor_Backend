/**************************************************************************
 * Project Name: Grocery API
 * File Name: Program.cs
 * Description: Entry point of the Grocery API application. Configures 
 * services, middleware, and application pipeline.
 * Author: Garry Sangha
 * Created On: 2024-12-24
 * Last Modified By: Garry Sangha
 * Last Modified On: 2024-12-29
 * Version: 1.2.0
 *************************************************************************/

#region Using Directives
using grocery_api.Database;
using grocery_api.Models.Repositories;
using grocery_api.Models.Repositories.Interfaces;
#endregion

#region Database Initialization

// Run database startup scripts and migrations
DatabaseStartup.RunDatabaseStartupScript();
EvolveInit.SyncDb();

/* 
 * Call the LoadDataFromCsv static method to preload data for "Chicken breasts", "Eggs", "Ketchup", "Onions", "Milk" into 
 * product_prices table from the CSV ( data needed by math model to predict the price for next month)
 */
RecordTableRepository.LoadDataFromCsv(
    "Host=localhost;Port=5432;Database=GroceryDB;User Id=superuser;Password=admin;",
    "./Database/migrations/1810024501-eng.csv"
);

#endregion

#region Application Initialization

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add scoped services
builder.Services.AddScoped<IGroceryItemRepository, GroceryItemRepository>();
builder.Services.AddScoped<IRecordTableRepository, RecordTableRepository>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

#endregion

#region App start Configuration

// Enable Swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

#endregion