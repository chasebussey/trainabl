using System.ComponentModel.DataAnnotations.Schema;

namespace Trainabl.Shared.Models;

[Table(name:"Metrics")]
public class Metric
{
	public Guid Id { get; set; }
	public Guid ClientProfileId { get; set; }
	public string Name { get; set; }
	public double Value { get; set; }
	public DateTime CreatedUTC { get; set; }
}