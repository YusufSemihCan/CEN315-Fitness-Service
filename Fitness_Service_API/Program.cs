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
// SECURITY MIDDLEWARE (OWASP ZAP FIX - Requirement 5.9)
// =================================================================
// We insert this FIRST so every request gets these headers immediately.
app.Use(async (context, next) =>
{
    // Prevents browsers from MIME-sniffing a response away from the declared content-type
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    // Protects against Clickjacking attacks
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

// =================================================================
// MIDDLEWARE PIPELINE
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

// Keep this. For or Integration Tests if you add them later.
public partial class Program { }