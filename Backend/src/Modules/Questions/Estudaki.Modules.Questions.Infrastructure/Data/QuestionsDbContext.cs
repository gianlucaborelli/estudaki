using Estudaki.Modules.Questions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estudaki.Modules.Questions.Infrastructure.Data;

public class QuestionsDbContext : DbContext
{
    public QuestionsDbContext(DbContextOptions<QuestionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Area> Areas => Set<Area>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Questions");

        builder.Entity<Area>(entity =>
        {
            entity.ToTable("Areas", "Questions");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(a => a.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(a => a.Type)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.CreatedAt)
                .IsRequired();

            entity.HasIndex(a => new { a.Type, a.Name });
        });
    }
}
