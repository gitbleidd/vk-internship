using System.ComponentModel.DataAnnotations;

namespace VkInternship.App.Models;


public record AuthInfo(
    [Required] string Login, 
    [Required] string Password
    );