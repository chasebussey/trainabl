namespace Trainabl.Shared.Models;

public class Movement
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string TargetMuscleGroup { get; set; }
	public bool RequiresEquipment { get; set; }
}