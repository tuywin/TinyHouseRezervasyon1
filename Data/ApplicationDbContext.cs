using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TinyHouseRezervasyon.Models;

namespace TinyHouseRezervasyon.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ev> Evler { get; set; }
    public DbSet<EvFotograf> EvFotograflari { get; set; }
    public DbSet<Rezervasyon> Rezervasyonlar { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Ev>()
            .HasOne(e => e.EvSahibi)
            .WithMany()
            .HasForeignKey(e => e.EvSahibiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EvFotograf>()
            .HasOne(f => f.Ev)
            .WithMany(e => e.Fotograflar)
            .HasForeignKey(f => f.EvId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Rezervasyon>()
            .HasOne(r => r.Ev)
            .WithMany(e => e.Rezervasyonlar)
            .HasForeignKey(r => r.EvId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rezervasyon>()
            .HasOne(r => r.Kullanici)
            .WithMany()
            .HasForeignKey(r => r.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 