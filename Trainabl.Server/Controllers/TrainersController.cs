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

	[HttpGet("{trainerId:guid}/workoutNotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetWorkoutNotesByTrainer(Guid trainerId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();

		List<WorkoutNote> notes = await _context.Workouts.Where(x => x.TrainerProfileId == trainerId && x.WorkoutNotes != null && x.WorkoutNotes.Count > 0)
		                                        .SelectMany(x => x.WorkoutNotes)
		                                        .ToListAsync();

		return notes;
	}

	[HttpGet("{trainerId:guid}/workoutNotes/search")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> SearchWorkoutNotes(
		Guid trainerId, DateTime? startDate, DateTime? endDate, string? searchString)
	{
		ActionResult<IEnumerable<WorkoutNote>> notesByTrainer = await GetWorkoutNotesByTrainer(trainerId);

		if (notesByTrainer.Value is null) return NotFound();
		
		IEnumerable<WorkoutNote>? notes = notesByTrainer.Value;

		if (startDate != null) notes = notes.Where(x => x.CreatedDateUTC.Date >= startDate);
		if (endDate != null) notes   = notes.Where(x => x.CreatedDateUTC.Date <= endDate);
		if (string.IsNullOrEmpty(searchString)) return notes.ToList();
		
		// check workout name
		notes = await notes.ToAsyncEnumerable()
		                   .WhereAwait(
			                   async x => (await _context.Workouts.FindAsync(x.WorkoutId)).Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
			                              || x.ExerciseNotes.Any(y => 
				                                                     y.Exercise.MovementName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
			                                                          || y.Note.Contains(searchString, StringComparison.OrdinalIgnoreCase))
			                              || (await _context.ClientProfiles.FindAsync(x.ClientProfileId)).Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
		                   )
		                   .ToListAsync();

		return notes.ToList();
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
	
	#region Delete

	[HttpDelete("{trainerId:guid}/clients")]
	public async Task<IActionResult> DeleteAllClientsForTrainer(Guid trainerId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);

		if (trainer is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		if (!isAuthorizedForTrainer) return Forbid();

		try
		{
			await _context.Entry(trainer).Navigation("ClientProfiles").LoadAsync();
			
			// delete all the client workouts, due to TrainerID FK constraint, efcore won't cascade delete
			foreach (var client in trainer.ClientProfiles)
			{
				await _context.Entry(client).Navigation("Workouts").LoadAsync();
				_context.Workouts.RemoveRange(client.Workouts);
			}
			
			_context.ClientProfiles.RemoveRange(trainer.ClientProfiles);
			await _context.SaveChangesAsync();
			return Ok();
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}

	[HttpDelete("{trainerId:guid}/clients/{clientId:guid}")]
	public async Task<IActionResult> DeleteClientForTrainer(Guid trainerId, Guid clientId)
	{
		var user    = HttpContext.User;
		var trainer = await _context.TrainerProfiles.FindAsync(trainerId);
		var client  = await _context.ClientProfiles.FindAsync(clientId);

		if (trainer is null) return NotFound();
		if (client is null) return NotFound();

		var isAuthorizedForTrainer = await _accessControl.IsAuthorizedForTrainer(user, trainer);
		var isAuthorizedForClient  = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForTrainer || !isAuthorizedForClient) return Forbid();

		try
		{
			await _context.Entry(client).Navigation("Workouts").LoadAsync();
			_context.Workouts.RemoveRange(client.Workouts);
			_context.ClientProfiles.Remove(client);
			await _context.SaveChangesAsync();
			return Ok();
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
		
	}

	#endregion
}