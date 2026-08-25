var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Temporarily disabled while we are only using HTTP locally
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();