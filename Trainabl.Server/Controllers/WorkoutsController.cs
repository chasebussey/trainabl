using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Client;
using Trainabl.Server.Services;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
	private readonly ApplicationContext _context;
	private AccessControlService _accessControl;

	public WorkoutsController(ApplicationContext context, AccessControlService accessControl)
	{
		_context       = context;
		_accessControl = accessControl;
	}

	#region Get

	[Authorize(Roles="Admin")]
	[HttpGet]
	public async Task<IEnumerable<WorkoutDTO>> GetAllWorkouts()
	{
		return await _context.Workouts.Select(x => Workout.WorkoutToDto(x)).ToListAsync();
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<WorkoutDTO>> GetWorkoutById(Guid id)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(id);
		
		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();

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
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);
		
		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();
		
		return Ok(workout.WorkoutNotes);
	}

	[HttpGet("{workoutId:guid}/latestworkoutnote")]
	public async Task<ActionResult<WorkoutNote>> GetLatestWorkoutNote(Guid workoutId)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);
		
		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();

		return Ok(workout.WorkoutNotes.MaxBy(x => x.CreatedDateUTC));
	}

	#endregion
	
	#region Post

	[Authorize(Roles="Trainer")]
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
	
	[Authorize(Roles="Trainer")]
	[HttpPost("{workoutId:guid}/workoutnotes")]
	public async Task<IActionResult> CreateWorkoutNote(Guid workoutId, WorkoutNote workoutNote)
	{
		try
		{
			var user    = HttpContext.User;
			var workout = await _context.Workouts.FindAsync(workoutId);

			if (workout is null) return NotFound();

			var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
			if (!isAuthorizedForWorkout) return Forbid();
			
			workoutNote.CreatedDateUTC = DateTime.UtcNow;
			
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

	[Authorize(Roles="Trainer")]
	[HttpPut("{workoutId:guid}")]
	public async Task<IActionResult> UpdateWorkout(Guid workoutId, WorkoutDTO workoutDto)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);

		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();

		var updatedWorkout = Workout.WorkoutFromDto(workoutDto);


		workout.Name             = updatedWorkout.Name;
		workout.Exercises        = updatedWorkout.Exercises;
		workout.IsTemplate       = updatedWorkout.IsTemplate;
		workout.IsDraft          = updatedWorkout.IsDraft;
		workout.Description      = updatedWorkout.Description;
		workout.WorkoutType      = updatedWorkout.WorkoutType;
		workout.TrainerProfileId = updatedWorkout.TrainerProfileId;
		workout.ClientProfileId  = updatedWorkout.ClientProfileId;

		if (workout.WorkoutNotes is { Count: > 0 } && !workout.WorkoutNotes.Contains(workoutDto.LatestNote))
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

	[Authorize(Roles="Trainer")]
	[HttpPut("{workoutId:guid}/workoutnotes/{noteId:guid}")]
	public async Task<IActionResult> UpdateWorkoutNote(Guid workoutId, Guid noteId, WorkoutNote updatedNote)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);

		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();
		
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

	[Authorize(Roles="Trainer")]
	[HttpDelete("{workoutId:guid}/workoutnotes")]
	public async Task<IActionResult> DeleteWorkoutNotes(Guid workoutId)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);

		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();

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

	[Authorize(Roles="Trainer")]
	[HttpDelete("{workoutId:guid}/workoutnotes/{noteId:guid}")]
	public async Task<IActionResult> DeleteWorkoutNoteById(Guid workoutId, Guid noteId)
	{
		var user    = HttpContext.User;
		var workout = await _context.Workouts.FindAsync(workoutId);

		if (workout is null) return NotFound();

		var isAuthorizedForWorkout = await _accessControl.IsAuthorizedForWorkout(user, workout);
		if (!isAuthorizedForWorkout) return Forbid();

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