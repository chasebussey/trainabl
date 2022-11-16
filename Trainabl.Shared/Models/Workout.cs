namespace Trainabl.Shared.Models;

public class Workout
{
	
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<Exercise> Exercises { get; set; }
	public bool IsTemplate { get;set; }
	public WorkoutType WorkoutType { get; set; }
	
	public string TrainerId { get; set; }
	public Guid? ClientProfileId { get; set; }
	//
	// public Trainer Trainer { get; set; }
	// public ClientProfile ClientProfile { get; set; }
	//
	// public List<WorkoutNote>? WorkoutNotes { get; set; }
}

public enum WorkoutType
{
	Standard,
	Circuit
}
