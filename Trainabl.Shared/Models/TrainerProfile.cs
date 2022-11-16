namespace Trainabl.Shared.Models;

public class TrainerProfile
{
	public Guid Id { get; set; }
	
	public string Name { get; set; }
	public string Email { get; set; }
	
	public List<Workout> Workouts { get; set; }
	public List<ClientProfile> ClientProfiles { get; set; }
}