using System.ComponentModel.DataAnnotations;
using VkInternship.Data.Entities;

namespace VkInternship.App.Models;

public record UserRegistrationInfo(
    [Required] string Login,
    [Required] string Password,
    [Required] string Group
    );