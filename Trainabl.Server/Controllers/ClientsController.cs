using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Trainabl.Server.Services;

namespace Trainabl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
	private readonly ApplicationContext _context;
	private AccessControlService _accessControl;

	public ClientsController(ApplicationContext context, AccessControlService accessControl)
	{
		_context       = context;
		_accessControl = accessControl;
	}

	#region Get
	
	[HttpGet("{clientId:guid}/latestmetrics")]
	public async Task<ActionResult<IEnumerable<Metric>>> GetLatestMetrics(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);
		
		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();

		List<Metric>        metrics     = client.Metrics;
		IEnumerable<string> metricNames = metrics.DistinctBy(x => x.Name).Select(x => x.Name);
		List<Metric> latestMetrics = metricNames.Select(metricName => metrics.Where(x => x.Name == metricName)
		                                                                     .OrderByDescending(x => x.CreatedUTC)
		                                                                     .First())
		                                        .ToList();

		return latestMetrics;
	}

	[HttpGet("{clientId:guid}/workouts")]
	public async Task<ActionResult<IEnumerable<WorkoutDTO>>> GetWorkoutsByClient(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();
		
		List<WorkoutDTO> workouts = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                          .Select(x => Workout.WorkoutToDto(x))
		                                          .ToListAsync();
		return workouts;
	}

	[HttpGet("{clientId:guid}/workoutnotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetAllWorkoutNotesForClient(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();
		
		List<WorkoutNote> notes = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                        .Where(x => x.WorkoutNotes != null && x.WorkoutNotes.Count > 0)
		                                        .SelectMany(x => x.WorkoutNotes)
		                                        .ToListAsync();

		return notes;
	}
	
	[HttpGet("{clientId:guid}/workoutNotes/search")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> SearchWorkoutNotes(
		Guid clientId, DateTime? startDate, DateTime? endDate, string? searchString)
	{
		ActionResult<IEnumerable<WorkoutNote>> notesForClient = await GetAllWorkoutNotesForClient(clientId);

		if (notesForClient.Value is null) return NotFound();
		
		IEnumerable<WorkoutNote>? notes = notesForClient.Value;

		if (startDate != null) notes = notes.Where(x => x.CreatedDateUTC.Date >= startDate);
		if (endDate != null) notes   = notes.Where(x => x.CreatedDateUTC.Date <= endDate);
		if (string.IsNullOrEmpty(searchString)) return notes.ToList();
		
		// check workout name
		notes = await notes.ToAsyncEnumerable()
		                   .WhereAwait(
			                   async x => (await _context.Workouts.FindAsync(x.WorkoutId)).Name.Contains(searchString))
		                   .ToListAsync();

		// check ExerciseNotes
		notes = notes.Where(
			x => x.ExerciseNotes.Any(y => y.Exercise.MovementName.Contains(searchString) ||
			                              y.Note.Contains(searchString)));

		return notes.ToList();
	}

	[HttpGet("{clientId:guid}/latestworkoutnotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetLatestWorkoutNotesForClient(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();
		
		List<WorkoutNote> notes = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                        .Where(x => x.WorkoutNotes != null && x.WorkoutNotes.Count > 0)
		                                        .Select(x => x.WorkoutNotes.MaxBy(y => y.CreatedDateUTC))
		                                        .ToListAsync();

		return Ok(notes);
	}

	[HttpGet("{clientId:guid}/metrics")]
	public async Task<ActionResult<IEnumerable<Metric>>> GetAllMetricsForClient(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();

		List<Metric> metrics = await _context.Metrics.Where(x => x.ClientProfileId == clientId)
		                                     .ToListAsync();

		return metrics;
	}

	[HttpGet("{clientId:guid}/metrics/search")]
	public async Task<ActionResult<IEnumerable<Metric>>> SearchClientMetrics(DateTime? startDate, DateTime? endDate, Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();

		IEnumerable<Metric> metrics = _context.Metrics.Where(x => x.ClientProfileId == clientId);
		
		if (startDate != null) metrics = metrics.Where(x => x.CreatedUTC.Date >= startDate);
		if (endDate != null) metrics   = metrics.Where(x => x.CreatedUTC.Date <= endDate);

		return metrics.ToList();
	}

	#endregion

	#region Post
	
	[HttpPost]
	public async Task<IActionResult> CreateClient(ClientProfileDTO clientDto)
	{
		var client = ClientProfile.ClientProfileFromDto(clientDto);
		if (client.Id == Guid.Empty)
		{
			client.Id = Guid.NewGuid();
		}
		
		// fix metrics, if any are new we need to assign guids
		foreach (var metric in client.Metrics.Where(metric => metric.Id == Guid.Empty))
		{
			metric.Id = Guid.NewGuid();
		}

		_context.ClientProfiles.Add(client);

		try
		{
			await _context.SaveChangesAsync();
			return Created(client.Id.ToString(), client);
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}
	
	#endregion
	
	#region Delete

	[Authorize(Roles="Trainer")]
	[HttpDelete("{clientId:guid}/workoutnotes")]
	public async Task<IActionResult> DeleteAllWorkoutNotesForClient(Guid clientId)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(clientId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();

		var workouts = await _context.Workouts.Where(x => x.ClientProfileId == clientId).ToListAsync();

		workouts.ForEach(x => x.WorkoutNotes.Clear());

		try
		{
			await _context.SaveChangesAsync();
			return Ok();
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}
	
	#endregion
	
	#region Helpers

	
	#endregion
}