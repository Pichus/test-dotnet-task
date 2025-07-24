using Microsoft.EntityFrameworkCore;
using TestDotnetTask.Database;
using TestDotnetTask.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TestDotnetTaskContext>(
    options => options.UseInMemoryDatabase("TestDotnetTaskDatabase"));

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument();

builder.Services.AddScoped<IMeetingSchedulerService, MeetingSchedulerService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();

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