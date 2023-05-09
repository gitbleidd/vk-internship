using System.ComponentModel.DataAnnotations;
using VkInternship.Data.Entities;

namespace VkInternship.App.Models;

public record UserRegistrationInfo
{
    [Required] public required string Login { get; init; }
    [Required] public required string Password { get; init; }
    public string Group { get; init; } = UserGroup.Group.User.ToString();
}