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
	public DbSet<TrainerProfile> TrainerProfiles { get; set; }
	public DbSet<ClientProfile> ClientProfiles { get; set; }
	public DbSet<Metric> Metrics { get; set; }
	
	public DbSet<UserSettings> UserSettings { get; set; }
	public DbSet<WorkoutNote> WorkoutNotes { get; set; }

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

		builder.Entity<Workout>()
		       .Navigation(x => x.WorkoutNotes)
		       .AutoInclude();

		builder.Entity<WorkoutNote>()
		       .Property(x => x.ExerciseNotes)
		       .HasConversion(
			       x => JsonSerializer.Serialize(x, (JsonSerializerOptions)null),
			       x => JsonSerializer.Deserialize<List<ExerciseNote>>(x, (JsonSerializerOptions)null)
		       );

		builder.Entity<ClientProfile>()
		       .Navigation(x => x.Metrics)
		       .AutoInclude();

		builder.Entity<Movement>()
		       .Property(x => x.Tags)
		       .HasConversion(
			       listTags => string.Join(",", listTags),
			       stringTags => stringTags.Split(",", StringSplitOptions.None).ToList()
			    );
	}
}