namespace Trainabl.Shared.Models;

public class Movement
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public MuscleGroup PrimaryMuscleGroup { get; set; }
	public MuscleGroup? SecondaryMuscleGroup { get; set; }
	public bool RequiresEquipment { get; set; }
	public List<string> Tags { get; set; }
	
	public Guid CreatedBy { get; set; }
	public Guid LastModifiedBy { get; set; }
	public DateTime CreatedDateUTC { get; set; }
	public DateTime LastModifiedUTC { get; set; }
}