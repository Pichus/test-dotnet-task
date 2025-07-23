using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TestDotnetTaskContext>(
    options => options.UseInMemoryDatabase("TestDotnetTaskDatabase"));

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();