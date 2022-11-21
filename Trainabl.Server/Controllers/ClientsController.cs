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
}