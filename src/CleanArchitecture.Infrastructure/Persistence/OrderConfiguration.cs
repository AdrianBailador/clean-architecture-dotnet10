using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence;

/// <summary>
/// Every ORM-mapping decision for Order lives here, isolated from the
/// Domain project. Order itself never references EF Core.
/// </summary>
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(Order.Lines))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        var lines = builder.OwnsMany(o => o.Lines);
        lines.WithOwner().HasForeignKey("OrderId");
        lines.Property<int>("Id");
        lines.HasKey("Id");
        lines.Property(l => l.ProductName).IsRequired().HasMaxLength(200);
        lines.Property(l => l.Quantity).IsRequired();

        var unitPrice = lines.OwnsOne(l => l.UnitPrice);
        unitPrice.Property(m => m.Amount).HasPrecision(18, 2);
        unitPrice.Property(m => m.Currency).HasMaxLength(3);
    }
}
