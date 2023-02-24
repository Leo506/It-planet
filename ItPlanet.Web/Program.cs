using ItPlanet.Infrastructure.DatabaseContext;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Repositories.VisitedPoint;
using ItPlanet.Web.Auth;
using ItPlanet.Web.Converters;
using ItPlanet.Web.Extensions;
using ItPlanet.Web.Services.Account;
using ItPlanet.Web.Services.Animal;
using ItPlanet.Web.Services.AnimalType;
using ItPlanet.Web.Services.Auth;
using ItPlanet.Web.Services.LocationPoint;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()));
var connectionString = builder.Configuration.GetConnectionString("postgres");
builder.Services
    .AddDbContext<ApiDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString))
    .AddTransient<IAccountService, AccountService>()
    .AddTransient<IAnimalService, AnimalService>()
    .AddTransient<IAnimalTypeService, AnimalTypeService>()
    .AddTransient<ILocationPointService, LocationPointService>()
    .AddTransient<IHeaderAuthenticationService, HeaderAuthenticationService>()
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<IAnimalRepository, AnimalRepository>()
    .AddTransient<IAnimalTypeRepository, AnimalTypeRepository>()
    .AddTransient<ILocationPointRepository, LocationPointRepository>()
    .AddTransient<IVisitedPointsRepository, VisitedPointRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("Header")
    .AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>("Header", null);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();