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
	public async Task<IEnumerable<WorkoutDTO>> GetAllWorkouts()
	{
		return await _context.Workouts.Select(x => Workout.WorkoutToDto(x)).ToListAsync();
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<WorkoutDTO>> GetWorkoutById(Guid id)
	{
		var workout = await _context.Workouts.FindAsync(id);
		if (workout is null)
		{
			return NotFound();
		}

		return Ok(Workout.WorkoutToDto(workout));
	}

	[HttpGet("templates")]
	public async Task<IEnumerable<WorkoutDTO>> GetWorkoutTemplates()
	{
		return await _context.Workouts.Where(x => x.IsTemplate)
		                     .Select(x => Workout.WorkoutToDto(x))
		                     .ToListAsync();
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
			return Created(workout.Id.ToString(), Workout.WorkoutToDto(workout));
		}
		catch (Exception e)
		{
			return BadRequest();
		}
	}
	#endregion

	#region Put

	[HttpPut("{workoutId:guid}")]
	public async Task<IActionResult> UpdateWorkout(Guid workoutId, WorkoutDTO workoutDto)
	{
		var updatedWorkout = Workout.WorkoutFromDto(workoutDto);
		var workout        = await _context.Workouts.FindAsync(workoutId);

		if (workout is null)
		{
			return NotFound();
		}

		workout.Name             = updatedWorkout.Name;
		workout.Exercises        = updatedWorkout.Exercises;
		workout.IsTemplate       = updatedWorkout.IsTemplate;
		workout.IsDraft          = updatedWorkout.IsDraft;
		workout.Description      = updatedWorkout.Description;
		workout.WorkoutType      = updatedWorkout.WorkoutType;
		workout.TrainerProfileId = updatedWorkout.TrainerProfileId;
		workout.ClientProfileId  = updatedWorkout.ClientProfileId;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException) when (!_context.Workouts.Any(x => x.Id == workoutId))
		{
			return NotFound();
		}

		return NoContent();
	}
	

	#endregion
}