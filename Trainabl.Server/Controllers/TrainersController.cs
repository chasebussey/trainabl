using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Client.Shared;
using Trainabl.Server.Services;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[Authorize(Roles="Trainer")]
[ApiController]
[Route("api/[controller]")]
public class TrainersController : ControllerBase
{
	private readonly ApplicationContext _context;
	private AccessControlService _accessControl;

	public TrainersController(ApplicationContext context, AccessControlService accessControl)
	{
		_context       = context;
		_accessControl = accessControl;
	}

	#region Get

	[Authorize(Roles="Admin")]
	[HttpGet]
	public Task<IEnumerable<TrainerProfile>> GetAllTrainers()
	{
		return Task.FromResult(_context.TrainerProfiles.AsEnumerable());
	}

	[HttpGet("{email}")]
	public async Task<ActionResult<TrainerProfile>> GetTrainerByEmail(string email)
	{
		var user   = HttpContext.User;
		var result = await _context.TrainerProfiles.FirstOrDefaultAsync(x => x.Email.Equals(email));

		if (result is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, result);
		if (!isAuthorizedForTrainer) return Forbid();

		return result;
	}

	[HttpGet("{trainerId:guid}/workouts")]
	public async Task<ActionResult<IEnumerable<WorkoutDTO>>> GetWorkoutsByTrainer(Guid trainerId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();
		
		List<WorkoutDTO> workouts = await _context.Workouts.Where(x => x.TrainerProfileId == trainerId)
		                                       .Select(x => Workout.WorkoutToDto(x))
		                                       .ToListAsync();
		return workouts;
	}

	[HttpGet("{trainerId:guid}/clients")]
	public async Task<ActionResult<IEnumerable<ClientProfileDTO>>> GetClientsByTrainer(Guid trainerId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();
		
		List<ClientProfileDTO> clients = await _context.ClientProfiles.Where(x => x.TrainerProfileId == trainerId)
		                                            .Select(x => ClientProfile.ClientProfileToDto(x))
		                                           .ToListAsync();
		return clients;
	}

	[HttpGet("{trainerId:guid}/clients/{clientId:guid}")]
	public async Task<IActionResult> GetClientByTrainerAndId(Guid trainerId, Guid clientId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();
		
		var client = await _context.ClientProfiles.FindAsync(clientId);
		
		if (client is null)
		{
			return NotFound();
		}

		if (client.TrainerProfileId != trainerId)
		{
			return Forbid();
		}

		return Ok(ClientProfile.ClientProfileToDto(client));
	}

	[HttpGet("{trainerId:guid}/settings")]
	public async Task<ActionResult<UserSettings?>> GetUserSettingForTrainer(Guid trainerId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();
		
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