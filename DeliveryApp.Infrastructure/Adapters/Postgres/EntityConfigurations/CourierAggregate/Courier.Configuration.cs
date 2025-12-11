using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;

class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder
            .OwnsOne(entity => entity.Location, b =>
            {
                b.Property(x => x.X).HasColumnName("location_x").IsRequired();
                b.Property(x => x.Y).HasColumnName("location_y").IsRequired();
            });
    }
}