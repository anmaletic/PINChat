namespace PINChat.Persistence.Db.Entities;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.ContactUserId)
            .IsRequired();

        // 'User' is the ApplicationUser who owns the contact list
        builder.HasOne(c => c.User)
            .WithMany(u => u.MyContacts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 'ContactUser' is the ApplicationUser being added as a contact
        builder.HasOne(c => c.ContactUser)
            .WithMany(u => u.AddedByOthers)
            .HasForeignKey(c => c.ContactUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.UserId, c.ContactUserId }).IsUnique();
    }
}