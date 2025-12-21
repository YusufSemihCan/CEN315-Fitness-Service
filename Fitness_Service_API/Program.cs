using Fitness_Service_API.Infrastructure; // Import the new class
using Fitness_Service_API.Services;

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// SERVICE REGISTRATION (Moved to Testable Method)
// =================================================================
// This single line replaces all the builder.Services.Add... calls
builder.Services.AddApplicationServices();

var app = builder.Build();

// =================================================================
// MIDDLEWARE PIPELINE (Remains in Program.cs)
// =================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Keep this! It is still good practice for Integration Tests if you add them later.
public partial class Program { }