using System.ComponentModel.DataAnnotations;
using ItPlanet.Domain.Models;

namespace ItPlanet.Domain.ValidationAttributes;

public class RoleAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string role)
            return false;

        return role is Role.Admin or Role.Chipper or Role.User;
    }
}