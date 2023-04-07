dotnet ef dbcontext scaffold 
"User Id=admin;Password=password;Host=localhost;Port=5432;Database=Test" 
"Npgsql.EntityFrameworkCore.PostgreSQL" 
--context ApiDbContext --context-dir DatabaseContext --output-dir ..\ItPlanet.Domain\Models\ --force