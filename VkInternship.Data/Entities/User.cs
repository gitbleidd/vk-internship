namespace VkInternship.Data.Entities;

public class User
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public required UserGroup UserGroup { get; set; }
    public required UserState UserState { get; set; }
}