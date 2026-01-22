namespace Ecommerce.Infrastructure.Congfiguration;
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
         builder.ToTable("Customer");
         builder.HasKey(c => c.Id);

         builder.Property(c =>c.Name).IsRequired().HasMaxLength(250);
        

         builder.HasMany(c =>c.Orders)
                .WithOne(o =>o.Customer)
                .HasForeignKey(o =>o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(Customer.Orders))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}