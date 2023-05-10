namespace VkInternship.Data.Entities;

public class UserGroup
{
    public enum Group
    {
        Admin = 1,
        User
    } 
    
    public int Id { get; set; }
    public Group Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; } = new();
}