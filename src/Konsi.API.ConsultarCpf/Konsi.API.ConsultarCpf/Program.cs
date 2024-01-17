using Konsi.API.ExternalServices.AppSettings;
using Konsi.API.ExternalServices.Interfaces;
using Konsi.API.ExternalServices.Services;
using Konsi.Domain.Interfaces;
using Konsi.Infrastructure.Elasticsearch.Configuration;
using Konsi.Infrastructure.Elasticsearch.Data;
using Konsi.Infrastructure.Messaging.Configuration;
using Konsi.Infrastructure.Messaging.RabbitMQ;
using Konsi.Infrastructure.Redis.Configuration;
using Konsi.Infrastructure.Redis.Data;
using Nest;
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


var elasticsearchSettings = builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchSettings>();
var settings = new ConnectionSettings(new Uri(elasticsearchSettings.Uri));
var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);
builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<CacheService>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();
builder.Services.AddTransient<IKonsiService, KonsiService>();
builder.Services.AddLogging();


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
