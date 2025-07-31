namespace PINChat.Persistence.Db.Entities;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.RecipientId)
            .IsRequired();

        builder.Property(m => m.Content)
            .HasMaxLength(2000);
        
        builder.HasOne(m => m.Sender)
            .WithMany() // ApplicationUser can send many messages
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.Recipient)
            .WithMany() // ApplicationUser can receive many messages
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}