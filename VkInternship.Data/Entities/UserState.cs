namespace VkInternship.Data.Entities;

public class UserState
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; } = new();
}