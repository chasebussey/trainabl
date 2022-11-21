namespace Trainabl.Shared.Models;

public class Metric
{
	public Guid Id { get; set; }
	public Guid ClientProfileId { get; set; }
	public string Name { get; set; }
	public double Value { get; set; }
	public string? Unit { get; set; }
	public DateTime CreatedUTC { get; set; }
}