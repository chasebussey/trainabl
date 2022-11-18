using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Client;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
	private readonly ApplicationContext _context;

	public WorkoutsController(ApplicationContext context)
	{
		_context = context;
	}

	#region Get

	[HttpGet]
	public Task<IEnumerable<Workout>> GetAllWorkouts()
	{
		return Task.FromResult(_context.Workouts.AsEnumerable());
	}

	#endregion
	
	#region Post

	[HttpPost]
	public async Task<IActionResult> CreateWorkout(WorkoutDTO workoutDto)
	{
		try
		{
			var workout = Workout.WorkoutFromDto(workoutDto);
			_context.Workouts.Add(workout);

			await _context.SaveChangesAsync();
			return Created(workout.Id.ToString(), workout);
		}
		catch (Exception e)
		{
			return BadRequest();
		}
	}
	#endregion
}