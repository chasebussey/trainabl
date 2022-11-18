namespace Trainabl.Shared.Models;

public class Workout
{
	
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<Exercise> Exercises { get; set; }
	public bool IsTemplate { get; set; }
	public bool IsDraft { get; set; } = true;
	public string? Description { get; set; }
	public WorkoutType WorkoutType { get; set; }
	
	public Guid TrainerProfileId { get; set; }
	public Guid? ClientProfileId { get; set; }
	
	public TrainerProfile TrainerProfile { get; set; }
	public ClientProfile? ClientProfile { get; set; }
	
	// TODO: Implement WorkoutNotes
	// public List<WorkoutNote>? WorkoutNotes { get; set; }

	public static Workout WorkoutFromDto(WorkoutDTO dto)
	{
		Workout workout = new()
		{
			Name             = dto.Name,
			Exercises        = dto.Exercises,
			IsTemplate       = dto.IsTemplate,
			IsDraft          = dto.IsDraft,
			Description      = dto.Description,
			WorkoutType      = dto.WorkoutType,
			TrainerProfileId = dto.TrainerProfileId,
			ClientProfileId  = dto.ClientProfileId
		};
		return workout;
	}
}

public enum WorkoutType
{
	Standard,
	Circuit
}
