using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
	private readonly ApplicationContext _context;

	public ClientsController(ApplicationContext context)
	{
		_context = context;
	}

	#region Get
	
	[HttpGet("{clientId:guid}/latestmetrics")]
	public async Task<IEnumerable<Metric>> GetLatestMetrics(Guid clientId)
	{
		var metrics     = await _context.Metrics.Where(x => x.ClientProfileId == clientId).ToListAsync();
		var metricNames = metrics.DistinctBy(x => x.Name).Select(x => x.Name);
		List<Metric> latestMetrics = metricNames.Select(metricName => metrics.Where(x => x.Name == metricName)
			                                                      .OrderByDescending(x => x.CreatedUTC)
			                                                      .First())
		                                              .ToList();

		return latestMetrics;
	}

	[HttpGet("{clientId:guid}/workouts")]
	public async Task<IEnumerable<WorkoutDTO>> GetWorkoutsByClient(Guid clientId)
	{
		List<WorkoutDTO> workouts = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                          .Select(x => Workout.WorkoutToDto(x))
		                                          .ToListAsync();
		return workouts;
	}

	[HttpGet("{clientId:guid}/workoutnotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetAllWorkoutNotesForClient(Guid clientId)
	{
		List<WorkoutNote> notes = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                        .Where(x => x.WorkoutNotes != null && x.WorkoutNotes.Count > 0)
		                                        .SelectMany(x => x.WorkoutNotes)
		                                        .ToListAsync();

		return Ok(notes);
	}

	[HttpGet("{clientId:guid}/latestworkoutnotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetLatestWorkoutNotesForClient(Guid clientId)
	{
		List<WorkoutNote> notes = await _context.Workouts.Where(x => x.ClientProfileId == clientId)
		                                        .Where(x => x.WorkoutNotes != null && x.WorkoutNotes.Count > 0)
		                                        .Select(x => x.WorkoutNotes.MaxBy(y => y.CreatedDateUTC))
		                                        .ToListAsync();

		return Ok(notes);
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

	[HttpDelete("{clientId:guid}/workoutnotes")]
	public async Task<IActionResult> DeleteAllWorkoutNotesForClient(Guid clientId)
	{
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
}