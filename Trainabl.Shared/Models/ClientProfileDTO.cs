namespace Trainabl.Shared.Models;

public class ClientProfileDTO
{
	public Guid Id { get; set; }
	public Guid TrainerProfileId { get; set; }

	public string Name { get; set; }
	public string Email { get; set; }
	public DateTime CreatedUTC { get; set; }
	public List<Metric> Metrics { get; set; }
}
