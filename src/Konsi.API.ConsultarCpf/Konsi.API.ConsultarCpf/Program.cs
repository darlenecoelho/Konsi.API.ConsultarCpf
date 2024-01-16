using Konsi.API.ExternalServices.AppSettings;
using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Services;
using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Messaging.Configuration;
using Konsi.Infrastructure.Messaging.RabbitMQ;
using Konsi.Infrastructure.Redis.Configuration;
using Konsi.Infrastructure.Redis.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<KonsiSettings>(builder.Configuration.GetSection("Konsi"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value ?? "localhost";

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<CacheService>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();
builder.Services.AddTransient<IKonsiService, KonsiService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
