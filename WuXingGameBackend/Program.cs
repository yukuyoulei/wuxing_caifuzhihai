using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System;
using WuXingGameBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置 net10
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WuXing Game API", Version = "v1" });
});

// 2. 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Allow any origin without credentials
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. 配置 MongoDB（端口 27018）
// Note: Make sure MongoDB is running on port 27018
// If MongoDB is not available, the application will still start but MongoDB features won't work
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    try
    {
        Console.WriteLine("Attempting to connect to MongoDB at mongodb://localhost:27018");
        var client = new MongoClient("mongodb://localhost:27018");
        // Test the connection
        client.ListDatabaseNames();
        Console.WriteLine("Successfully connected to MongoDB");
        return client;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to connect to MongoDB: " + ex.Message);
        // If MongoDB is not available, we can either:
        // 1. Return null and handle null checks in controllers
        // 2. Return a mock client for development
        // 3. Disable MongoDB features gracefully
        // For now, we'll return null and handle it in the controllers
        Console.WriteLine("MongoDB features will be disabled");
        return null;
    }
});

// 3. 配置 WebSocket
builder.Services.AddSignalR(options =>
{
    // Configure SignalR options
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Log when the GameHub is mapped
Console.WriteLine("Mapping GameHub to /gameHub");
app.MapHub<GameHub>("/gameHub");

// Log when the application is about to start
Console.WriteLine("Application starting...");

app.Run();