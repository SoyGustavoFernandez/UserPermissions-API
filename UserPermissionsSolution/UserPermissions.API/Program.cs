using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Data;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;
using UserPermissions.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["Elasticsearch:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "user-permissions-api-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<KafkaProducerService>(sp =>
{
    var bootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
    return new KafkaProducerService(bootstrapServers);
});

builder.Services.AddSingleton<ElasticsearchService>(sp =>
{
    var uri = builder.Configuration["Elasticsearch:Uri"];
    return new ElasticsearchService(uri);
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();

var app = builder.Build();

Log.Information("Application started");

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();