// src/Modules/Common/Modules.Common.Infrastructure/Outbox/OutboxMessageConfiguration.cs
namespace Modules.Common.Infrastructure.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Content).HasColumnType("jsonb").IsRequired(); // Leverage Postgres Native JSON
    }
}