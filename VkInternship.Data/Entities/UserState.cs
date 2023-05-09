namespace VkInternship.Data.Entities;

public class UserState
{
    public enum State
    {
        Active,
        Blocked
    }
    
    public int Id { get; set; }
    public required State Code { get; set; } = State.Active;
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; } = new();
}