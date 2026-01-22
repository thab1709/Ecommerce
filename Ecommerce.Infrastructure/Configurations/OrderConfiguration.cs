namespace Ecommerce.Infrastructure.Congfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(o => o.Id);

        builder.Property(o  => o.TotalAmount )
                .HasColumnType("demacial(18,3)")
                .IsRequired();
        builder.Property(o => o.Status)
                .IsRequired();
        builder.Property(o =>o.CreatedAt)
                .IsRequired();
      


    }


}