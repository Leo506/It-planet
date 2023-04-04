using ItPlanet.Domain.Models;
using ItPlanet.Infrastructure.Repositories.Account;

namespace ItPlanet.Web.Services.DatabaseFiller;

public class AccountFiller : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AccountFiller(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        
        if (await accountRepository.ExistAsync(1).ConfigureAwait(false))
            return;

        await accountRepository.CreateRangeAsync(
            new Domain.Models.Account()
            {
                FirstName = "adminFirstName",
                LastName = "adminLastName",
                Email = "admin@simbirsoft.com",
                Password = "qwerty123",
                Role = new Role()
                {
                    RoleName = "ADMIN"
                }
            },
            new Domain.Models.Account()
            {
                FirstName = "chipperFirstName",
                LastName = "chipperLastName",
                Email = "chipper@simbirsoft.com",
                Password = "qwerty123",
                Role = new Role()
                {
                    RoleName = "CHIPPER"
                }
            },
            new Domain.Models.Account()
            {
                FirstName = "userFirstName",
                LastName = "userLastName",
                Email = "user@simbirsoft.com",
                Password = "qwerty123",
                Role = new Role()
                {
                    RoleName = "USER"
                }
            }).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}