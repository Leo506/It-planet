namespace ItPlanet.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(int accountId) : base($"Account with id {accountId} was not found")
    {
    }
}