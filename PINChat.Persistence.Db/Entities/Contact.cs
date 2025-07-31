namespace PINChat.Persistence.Db.Entities;

public class Contact
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string ContactUserId { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ApplicationUser ContactUser { get; set; } = null!;
}