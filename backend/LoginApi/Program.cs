using LoginApi.Data;
using LoginApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enables controller support
builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();


// Enables Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enables HTTPS redirection
app.UseHttpsRedirection();

// Enables routing for API controllers
app.MapControllers();

app.Run();
