using SakuraJounin.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConnection = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddGrpcClient<SakuraGenin.Genin.GeninClient>(c =>
{
    c.Address = new Uri(builder.Configuration.GetConnectionString("Genin"));
});
builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
