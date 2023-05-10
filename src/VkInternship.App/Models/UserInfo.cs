namespace VkInternship.App.Models;

public record UserInfo
{
    public int Id { get; init; }
    public required string Login { get; init; }
    public DateTime CreatedDate { get; init; }
    public required string Group { get; init; }
}