using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

public class StoragePlaceConfiguration : IEntityTypeConfiguration<StoragePlace>
{
    public void Configure(EntityTypeBuilder<StoragePlace> builder)
    {
        builder
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(x => x.OrderId);
    }
}