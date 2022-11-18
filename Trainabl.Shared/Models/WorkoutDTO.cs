namespace Trainabl.Shared.Models;

public class WorkoutDTO
{
	public string Name { get; set; }
	public List<Exercise> Exercises { get; set; }
	public bool IsTemplate { get; set; }
	public bool IsDraft { get; set; } = true;
	public string? Description { get; set; }
	public WorkoutType WorkoutType { get; set; }
	
	public Guid TrainerProfileId { get; set; }
	public Guid? ClientProfileId { get; set; }
}