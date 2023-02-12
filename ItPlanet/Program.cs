using ItPlanet.Converters;
using ItPlanet.Database.DbContexts;
using ItPlanet.Database.Repositories.Account;
using ItPlanet.Database.Repositories.Animal;
using ItPlanet.Database.Repositories.AnimalType;
using ItPlanet.Services.Account;
using ItPlanet.Services.Animal;
using ItPlanet.Services.AnimalType;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()));
var connectionString = builder.Configuration.GetConnectionString("postgres");
builder.Services
    .AddDbContext<ApiDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString))
    .AddTransient<IAccountService, AccountService>()
    .AddTransient<IAnimalService, AnimalService>()
    .AddTransient<IAnimalTypeService, AnimalTypeService>()
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<IAnimalRepository, AnimalRepository>()
    .AddTransient<IAnimalTypeRepository, AnimalTypeRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();