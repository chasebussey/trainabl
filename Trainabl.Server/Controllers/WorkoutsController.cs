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

	[HttpGet("{workoutId:guid}/workoutnotes")]
	public async Task<ActionResult<IEnumerable<WorkoutNote>>> GetWorkoutNotes(Guid workoutId)
	{
		var workout = await _context.Workouts.FindAsync(workoutId);
		if (workout is null)
		{
			return NotFound();
		}

		return Ok(workout.WorkoutNotes);
	}

	[HttpGet("{workoutId:guid}/latestworkoutnote")]
	public async Task<ActionResult<WorkoutNote>> GetLatestWorkoutNote(Guid workoutId)
	{
		var workout = await _context.Workouts.FindAsync(workoutId);
		if (workout is null)
		{
			return NotFound();
		}

		return Ok(workout.WorkoutNotes.MaxBy(x => x.CreatedDateUTC));
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

	[HttpPost("{workoutId:guid}/workoutnotes")]
	public async Task<IActionResult> CreateWorkoutNote(Guid workoutId, WorkoutNote workoutNote)
	{
		try
		{
			var workout = await _context.Workouts.FindAsync(workoutId);
			workout.WorkoutNotes.Add(workoutNote);

			await _context.SaveChangesAsync();
			return Created(workoutNote.Id.ToString(), workoutNote);
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

		if (workout.WorkoutNotes != null && !workout.WorkoutNotes.Contains(workoutDto.LatestNote))
		{
			workout.WorkoutNotes.Add(workoutDto.LatestNote);
		}

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

	[HttpPut("{workoutId:guid}/workoutnotes/{noteId:guid}")]
	public async Task<IActionResult> UpdateWorkoutNote(Guid workoutId, Guid noteId, WorkoutNote updatedNote)
	{
		var note = await _context.WorkoutNotes.FindAsync(noteId);
		if (note is null)
		{
			return NotFound();
		}
		
		if (note.WorkoutId != workoutId)
		{
			return BadRequest();
		}

		note.ExerciseNotes = updatedNote.ExerciseNotes;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException) when (!_context.Workouts.Any(x => x.Id == noteId))
		{
			return NotFound();
		}

		return NoContent();
	}
	
	#endregion
	
	#region Delete

	[HttpDelete("{workoutId:guid}/workoutnotes")]
	public async Task<IActionResult> DeleteWorkoutNotes(Guid workoutId)
	{
		var workout = await _context.Workouts.FindAsync(workoutId);
		if (workout is null)
		{
			return NotFound();
		}

		workout.WorkoutNotes.Clear();

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

	[HttpDelete("{workoutId:guid}/workoutnotes/{noteId:guid}")]
	public async Task<IActionResult> DeleteWorkoutNoteById(Guid workoutId, Guid noteId)
	{
		var workout = await _context.Workouts.FindAsync(workoutId);
		if (workout is null)
		{
			return NotFound();
		}

		var note = workout.WorkoutNotes.Find(x => x.Id == noteId);
		if (note is null)
		{
			return NotFound();
		}

		workout.WorkoutNotes.Remove(note);

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