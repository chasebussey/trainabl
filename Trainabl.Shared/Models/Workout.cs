namespace Trainabl.Shared.Models;

public class Workout
{
	
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<Exercise> Exercises { get; set; }
	public bool IsTemplate { get; set; }
	public bool IsDraft { get; set; }
	public string? Description { get; set; }
	public WorkoutType WorkoutType { get; set; }
	
	public Guid TrainerProfileId { get; set; }
	public Guid? ClientProfileId { get; set; }
	
	public TrainerProfile TrainerProfile { get; set; }
	public ClientProfile? ClientProfile { get; set; }
	
	public List<WorkoutNote>? WorkoutNotes { get; set; }

	// WorkoutFromDto doesn't assign LatestNote, that'll have to be handled by the controller
	public static Workout WorkoutFromDto(WorkoutDTO dto) =>
		new()
		{
			Id               = dto.Id,
			Name             = dto.Name,
			Exercises        = dto.Exercises,
			IsTemplate       = dto.IsTemplate,
			IsDraft          = dto.IsDraft,
			Description      = dto.Description,
			WorkoutType      = dto.WorkoutType,
			TrainerProfileId = dto.TrainerProfileId,
			ClientProfileId  = dto.ClientProfileId,
		};

	public static WorkoutDTO WorkoutToDto(Workout workout)
	{
		var dto = new WorkoutDTO
		{
			Id               = workout.Id,
			Name             = workout.Name,
			Exercises        = workout.Exercises,
			IsTemplate       = workout.IsTemplate,
			IsDraft          = workout.IsDraft,
			Description      = workout.Description,
			WorkoutType      = workout.WorkoutType,
			TrainerProfileId = workout.TrainerProfileId,
			ClientProfileId  = workout.ClientProfileId,
		};

		if (workout.WorkoutNotes is not null && workout.WorkoutNotes.Count > 0)
		{
			dto.LatestNote = workout.WorkoutNotes.MaxBy(x => x.CreatedDateUTC);
		}

		return dto;
	}
}

public enum WorkoutType
{
	Standard,
	Circuit
}
