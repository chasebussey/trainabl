namespace Trainabl.Shared.Models;

public class Goal
{
	public Guid Id { get; set; }
	public Guid ClientProfileId { get; set; }
	public GoalType GoalType { get; set; }
	public bool IsComplete { get; set; }
	
	public string? Metric { get; set; }
	public string? Comparator { get; set; }
	public double? TargetValue { get; set; }
	
	public string? Description { get; set; }
	
	public DateTime? DeadlineUTC { get; set; }
	
	public DateTime CreatedDateUTC { get; set; }
	public DateTime LastModifiedUTC { get; set; }

	public override string ToString()
	{
		return GoalType switch
		{
			GoalType.Metric => $"{Metric} {Comparator} {TargetValue}",
			GoalType.Custom => Description
		};
	}
}

public enum GoalType
{
	Metric,
	Custom
}