using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MovementsController : ControllerBase
{
	private readonly ApplicationContext _context;

	public MovementsController(ApplicationContext context)
	{
		_context = context;
	}
	
	[Authorize(Roles="Trainer")]
	[HttpPost]
	public async Task<ActionResult<bool>> CreateMovement(Movement movement)
	{
		if (movement.Id == Guid.Empty) movement.Id                                = Guid.NewGuid();
		if (movement.CreatedDateUTC == DateTime.MinValue) movement.CreatedDateUTC = DateTime.UtcNow;
		movement.LastModifiedUTC = DateTime.UtcNow;
		
		_context.Movements.Add(movement);
		var results = await _context.SaveChangesAsync();

		return results > 0;
	}

	[HttpGet]
	public async Task<IEnumerable<Movement>> GetAllMovements()
	{
		return await _context.Movements.ToListAsync();
	}

	[HttpGet("{id:guid}")]
	public async Task<Movement?> GetMovement(Guid id)
	{
		return await _context.Movements.FindAsync(id);
	}

	[HttpGet("search")]
	public Task<IEnumerable<Movement>> SearchMovements(string? name = null, MuscleGroup? targetMuscleGroup = null, bool? requiresEquipment = null)
	{
		IEnumerable<Movement> matches = _context.Movements;
		if (name is not null)
		{
			matches = matches.Where(x => x.Name.Equals(name));
		}

		if (targetMuscleGroup is not null)
		{
			matches = matches.Where(x => x.PrimaryMuscleGroup == targetMuscleGroup || x.SecondaryMuscleGroup == targetMuscleGroup);
		}

		if (requiresEquipment is not null)
		{
			matches = matches.Where(x => x.RequiresEquipment == requiresEquipment);
		}

		return Task.FromResult(matches);
	}

	[Authorize(Roles="Trainer")]
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

		movement.Name                 = updatedMovement.Name;
		movement.PrimaryMuscleGroup   = updatedMovement.PrimaryMuscleGroup;
		movement.SecondaryMuscleGroup = updatedMovement.SecondaryMuscleGroup;
		movement.RequiresEquipment    = updatedMovement.RequiresEquipment;
		movement.Tags                 = updatedMovement.Tags;
		
		movement.LastModifiedUTC = DateTime.UtcNow;

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

	[Authorize(Roles="Trainer")]
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