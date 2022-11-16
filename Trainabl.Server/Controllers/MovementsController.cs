using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MovementsController : ControllerBase
{
	private readonly ApplicationContext _context;

	public MovementsController(ApplicationContext context)
	{
		_context = context;
	}
	
	[HttpPost]
	public async Task<ActionResult<bool>> CreateMovement(Movement movement)
	{
		_context.Movements.Add(movement);
		var results = await _context.SaveChangesAsync();

		return results > 0;
	}

	[HttpGet("all")]
	public async Task<IEnumerable<Movement>> GetAllMovements()
	{
		return await _context.Movements.ToListAsync();
	}

	[HttpGet("{id:guid}")]
	public async Task<Movement?> GetMovement(Guid id)
	{
		return await _context.Movements.FindAsync(id);
	}

	[HttpGet]
	public Task<IEnumerable<Movement>> SearchMovements(string? name = null, string? targetMuscleGroup = null, bool? requiresEquipment = null)
	{
		IEnumerable<Movement> matches = _context.Movements;
		if (name is not null)
		{
			matches = matches.Where(x => x.Name.Equals(name));
		}

		if (targetMuscleGroup is not null)
		{
			matches = matches.Where(x => x.TargetMuscleGroup.Equals(targetMuscleGroup));
		}

		if (requiresEquipment is not null)
		{
			matches = matches.Where(x => x.RequiresEquipment == requiresEquipment);
		}

		return Task.FromResult(matches);
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> UpdateMovement(Guid id, Movement updatedMovement)
	{
		if (id != updatedMovement.Id)
		{
			return BadRequest();
		}

		var movement = await _context.Movements.FindAsync(id);

		if (movement is null)
		{
			return NotFound();
		}

		movement.Name              = updatedMovement.Name;
		movement.TargetMuscleGroup = updatedMovement.TargetMuscleGroup;
		movement.RequiresEquipment = updatedMovement.RequiresEquipment;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException) when (!_context.Movements.Any(x => x.Id == id))
		{
			return NotFound();
		}

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> DeleteMovement(Guid id)
	{
		var movement = await _context.Movements.FindAsync(id);

		if (movement is null)
		{
			return NotFound();
		}

		_context.Movements.Remove(movement);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}