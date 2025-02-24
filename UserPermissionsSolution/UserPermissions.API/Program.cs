using Microsoft.EntityFrameworkCore;
using Serilog;
using UserPermissions.Application.Mappings;
using UserPermissions.Application.Queries;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Data;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;
using UserPermissions.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IKafkaProducerService>(sp =>
{
    var bootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
    return new KafkaProducerService(bootstrapServers);
});

builder.Services.AddSingleton<IElasticsearchService>(sp =>
{
    var uri = builder.Configuration["Elasticsearch:Uri"];
    return new ElasticsearchService(uri);
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(GetPermissionsQuery).Assembly));

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "UserPermissions.API",
        Version = "v1",
        Description = "A simple API to manage user permissions",
        Contact = new()
        {
            Name = "Gustavo Fernández",
            Email = "soygustavofernandez@gmail.com",
            Url = new("https://www.linkedin.com/in/soygustavofernandez/")
        }
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

Log.Information("Application started");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserPermissions API v1");
    });
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();