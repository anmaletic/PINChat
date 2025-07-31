namespace PINChat.Persistence.Db.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public byte[]? Avatar { get; set; }  
    public string? AvatarPath { get; set; }  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Contact> MyContacts { get; set; } = [];
    public ICollection<Contact> AddedByOthers { get; set; } = [];
}