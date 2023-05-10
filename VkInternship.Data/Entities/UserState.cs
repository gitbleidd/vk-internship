namespace VkInternship.Data.Entities;

public class UserState
{
    public enum State
    {
        Active = 1,
        Blocked
    }
    
    public int Id { get; set; }
    public State Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; } = new();
}