using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server;

public class ApplicationContext : DbContext
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