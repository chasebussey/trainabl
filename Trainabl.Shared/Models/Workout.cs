namespace Trainabl.Shared.Models;

public class Workout
{
	
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<Exercise> Exercises { get; set; }
	public bool IsTemplate { get;set; }
	public WorkoutType WorkoutType { get; set; }
	
	public Guid TrainerId { get; set; }
	public Guid? ClientProfileId { get; set; }
	
	public TrainerProfile TrainerProfile { get; set; }
	public ClientProfile ClientProfile { get; set; }
	
	// TODO: Implement WorkoutNotes
	// public List<WorkoutNote>? WorkoutNotes { get; set; }
}

public enum WorkoutType
{
	Standard,
	Circuit
}
