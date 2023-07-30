using System.Text.Json;
using System.Text.Json.Serialization;
using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.DatabaseContext;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.Area;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Repositories.Role;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;
using ItPlanet.Web.Auth;
using ItPlanet.Web.Converters;
using ItPlanet.Web.Extensions;
using ItPlanet.Web.Mapping;
using ItPlanet.Web.Services.Account;
using ItPlanet.Web.Services.Animal;
using ItPlanet.Web.Services.AnimalType;
using ItPlanet.Web.Services.Area;
using ItPlanet.Web.Services.Auth;
using ItPlanet.Web.Services.DatabaseFiller;
using ItPlanet.Web.Services.LocationPoint;
using ItPlanet.Web.Services.VisitedPoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Destructure.ByTransforming<Account>(account => JsonSerializer.Serialize(account))
    .WriteTo.Console()
    .WriteTo.Seq(serverUrl:"http://localhost:5341")
    .CreateLogger();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

#if INMEMORY
builder.Services.AddDbContext<ApiDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("API"));
#else
var connectionString = builder.Configuration.GetConnectionString("postgres");
builder.Services.AddDbContext<ApiDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
#endif

builder.Services
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog())
    .AddTransient<IAccountService, AccountService>()
    .AddTransient<IAnimalService, AnimalService>()
    .AddTransient<IAnimalTypeService, AnimalTypeService>()
    .AddTransient<ILocationPointService, LocationPointService>()
    .AddTransient<IHeaderAuthenticationService, HeaderAuthenticationService>()
    .AddTransient<IVisitedPointsService, VisitedPointsService>()
    .AddTransient<IAreaService, AreaService>()
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<IAnimalRepository, AnimalRepository>()
    .AddTransient<IAnimalTypeRepository, AnimalTypeRepository>()
    .AddTransient<ILocationPointRepository, LocationPointRepository>()
    .AddTransient<IVisitedPointRepository, VisitedPointRepository>()
    .AddTransient<IRoleRepository, RoleRepository>()
    .AddTransient<IAreaRepository, AreaRepository>()
    .AddAutoMapper(typeof(AutoMapperProfile))
    .AddHostedService<AccountFiller>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "It planet Web API",
        Version = "v2"
    });
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Provide email and password",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(options => options.DefaultScheme = "Basic")
    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("v2/swagger.json", "It planet"));
}

app.UseRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();