using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
	private readonly ApplicationContext _context;

	public TrainersController(ApplicationContext context)
	{
		_context = context;
	}

	#region Get

	[HttpGet]
	public Task<IEnumerable<TrainerProfile>> GetAllTrainers()
	{
		return Task.FromResult(_context.TrainerProfiles.AsEnumerable());
	}

	[HttpGet("{email}")]
	public async Task<ActionResult<TrainerProfile>> GetTrainerByEmail(string email)
	{
		var result = await _context.TrainerProfiles.FirstOrDefaultAsync(x => x.Email.Equals(email));
		if (result is not null)
		{
			return result;
		}
		else
		{
			return NotFound();
		}
	}

	[HttpGet("{trainerId:guid}/workouts")]
	public async Task<IEnumerable<WorkoutDTO>> GetWorkoutsByTrainer(Guid trainerId)
	{
		List<WorkoutDTO> workouts = await _context.Workouts.Where(x => x.TrainerProfileId == trainerId)
		                                       .Select(x => Workout.WorkoutToDto(x))
		                                       .ToListAsync();
		return workouts;
	}

	[HttpGet("{trainerId:guid}/clients")]
	public async Task<IEnumerable<ClientProfile>> GetClientsByTrainer(Guid trainerId)
	{
		List<ClientProfile> clients = await _context.ClientProfiles.Where(x => x.TrainerProfileId == trainerId)
		                                           .ToListAsync();
		return clients;
	}

	[HttpGet("{trainerId:guid}/settings")]
	public async Task<UserSettings?> GetUserSettingForTrainer(Guid trainerId)
	{
		return await _context.UserSettings.FirstOrDefaultAsync(x => x.UserId == trainerId);
	}
	
	#endregion
	
	#region Post

	[HttpPost]
	public async Task<IActionResult> CreateTrainer(TrainerProfile trainer)
	{
		_context.TrainerProfiles.Add(trainer);
		try
		{
			await _context.SaveChangesAsync();
			return Created(trainer.Id.ToString(), trainer);
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}

	#endregion
}