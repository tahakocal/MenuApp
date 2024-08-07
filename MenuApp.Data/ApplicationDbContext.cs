using MenuApp.Data.Entities;
using MenuApp.Data.Entities.Identity;
using MenuApp.Data.Entities.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace MenuApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Log> Logs { get; set; }
    public DbSet<Language> Language { get; set; }
    public DbSet<LocaleStringResource> LocaleStringResource { get; set; }
    public DbSet<MailSetting> MailSetting { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<Brand> Brand { get; set; }
    public DbSet<Category> Category { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var converter = new ValueConverter<string, string>(
            v => JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<string>>(v)),
            v => JsonConvert.DeserializeObject<List<string>>(v) != null
                ? JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<string>>(v))
                : v
        );

        builder.Entity<Product>()
            .Property(p => p.Images)
            .HasConversion(converter);

        RenameIdentityTables(builder);
    }

    private void RenameIdentityTables(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("dbo");
        builder.Entity<ApplicationUser>(entity => { entity.ToTable("Users"); });
        builder.Entity<ApplicationRole>(entity => { entity.ToTable("Roles"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
    }
}