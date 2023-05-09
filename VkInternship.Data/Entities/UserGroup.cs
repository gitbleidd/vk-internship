namespace VkInternship.Data.Entities;

public class UserGroup
{
    public enum Group
    {
        Admin,
        User
    } 
    
    public int Id { get; set; }
    public required Group Code { get; set; } = Group.User;
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; } = new();
}