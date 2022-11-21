namespace Trainabl.Shared.Models;

public class ClientProfile
{
	public Guid Id { get; set; }
	public Guid TrainerProfileId { get; set; }
	
	public string Name { get; set; }
	public string Email { get; set; }
	public UserSettings? UserSettings { get; set; }
	public DateTime CreatedUTC { get; set; }
	
	public List<Workout> Workouts { get; set; }

	public TrainerProfile TrainerProfile { get; set; }
	// TODO: Implement Metrics and Goals
	public List<Metric> Metrics { get; set; }
	// public List<Goal> Goals { get; set; }

	public static ClientProfileDTO ClientProfileToDto(ClientProfile client) =>
		new()
		{
			Id               = client.Id,
			TrainerProfileId = client.TrainerProfileId,
			Name             = client.Name,
			Email            = client.Email,
			Metrics          = client.Metrics,
			CreatedUTC       = client.CreatedUTC
		};

	public static ClientProfile ClientProfileFromDto(ClientProfileDTO dto) =>
		new()
		{
			Id               = dto.Id,
			TrainerProfileId = dto.TrainerProfileId,
			Name             = dto.Name,
			Email            = dto.Email,
			Metrics          = dto.Metrics,
			CreatedUTC       = dto.CreatedUTC
		};
}