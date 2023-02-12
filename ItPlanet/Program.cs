using ItPlanet.Database.DbContexts;
using ItPlanet.Database.Repositories.Account;
using ItPlanet.Services.Account;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("postgres");
builder.Services
    .AddDbContext<ApiDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString))
    .AddTransient<IAccountService, AccountService>()
    .AddScoped<IAccountRepository, AccountRepository>();

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