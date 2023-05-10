namespace VkInternship.Data.Entities;

public class User
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int GroupId { get; set; }
    public UserGroup Group { get; set; } = null!;
    public int StateId { get; set; }
    public UserState State { get; set; } = null!;
}