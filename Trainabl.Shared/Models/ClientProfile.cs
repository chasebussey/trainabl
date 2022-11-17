namespace Trainabl.Shared.Models;

public class ClientProfile
{
	public Guid Id { get; set; }
	public Guid TrainerProfileId { get; set; }
	
	public string Name { get; set; }
	public string Email { get; set; }
	public UserSettings? UserSettings { get; set; }
	
	public List<Workout> Workouts { get; set; }

	public TrainerProfile TrainerProfile { get; set; }
	// TODO: Implement Metrics and Goals
	// public List<Metric> Metrics { get; set; }
	// public List<Goal> Goals { get; set; }
}