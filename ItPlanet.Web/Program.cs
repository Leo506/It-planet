using ItPlanet.Infrastructure.DatabaseContext;
using ItPlanet.Infrastructure.Repositories.Account;
using ItPlanet.Infrastructure.Repositories.Animal;
using ItPlanet.Infrastructure.Repositories.AnimalType;
using ItPlanet.Infrastructure.Repositories.LocationPoint;
using ItPlanet.Infrastructure.Services.Account;
using ItPlanet.Infrastructure.Services.Animal;
using ItPlanet.Infrastructure.Services.AnimalType;
using ItPlanet.Infrastructure.Services.Auth;
using ItPlanet.Infrastructure.Services.LocationPoint;
using ItPlanet.Web.Auth;
using ItPlanet.Web.Converters;
using ItPlanet.Web.Extensions;
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
    .AddTransient<ILocationPointRepository, LocationPointRepository>();

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