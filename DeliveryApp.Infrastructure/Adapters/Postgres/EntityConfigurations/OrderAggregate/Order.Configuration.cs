using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;

public class OrderConfiguration  : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .OwnsOne(entity => entity.Location, b =>
            {
                b.Property(x => x.X).HasColumnName("location_x").IsRequired();
                b.Property(x => x.Y).HasColumnName("location_y").IsRequired();
            });
        
        builder
            .OwnsOne(entity => entity.Status, b =>
            {
                b.Property(x => x.Name).HasColumnName("status").IsRequired();
            });
    }
}