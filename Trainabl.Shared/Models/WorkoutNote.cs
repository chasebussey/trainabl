namespace Trainabl.Shared.Models;

public class WorkoutNote
{
	public Guid Id { get; set; }
	public DateTime CreatedDateUTC { get; set; }
	public Guid WorkoutId { get; set; }
	public List<ExerciseNote> ExerciseNotes { get; set; }
}