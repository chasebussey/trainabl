using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server;

public class ApplicationContext : IdentityDbContext
{
	public ApplicationContext(DbContextOptions<ApplicationContext> options)
		: base(options)
	{
	}

	public DbSet<Movement> Movements { get; set; }
	public DbSet<Workout> Workouts { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		
        // Modifying the Identity model to use better table names
        builder.Entity<IdentityUser>(e =>
        {
            e.ToTable(name: "Users");
        });
        builder.Entity<IdentityRole>(e =>
        {
            e.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<string>>(e =>
        {
            e.ToTable(name: "UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(e =>
        {
            e.ToTable(name: "UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(e =>
        {
            e.ToTable(name: "UserLogins");
        });
        builder.Entity<IdentityRoleClaim<string>>(e =>
        {
            e.ToTable(name: "RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(e =>
        {
            e.ToTable(name: "UserTokens");
        });
		
		// I don't want to store each exercise from a workout as its own row
		// so let's do a JSON field
		builder.Entity<Workout>()
		            .Property(x => x.Exercises)
		            .HasConversion(
			            e => JsonSerializer.Serialize(e, (JsonSerializerOptions)null),
			            e => JsonSerializer.Deserialize<List<Exercise>>(e, (JsonSerializerOptions)null)
		            );
	}
}