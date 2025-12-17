using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置 net10
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WuXing Game API", Version = "v1" });
});

// 2. 配置 MongoDB（端口 27018）
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient("mongodb://localhost:27018");
});

// 3. 配置 WebSocket
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();