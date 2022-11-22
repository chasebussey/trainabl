using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
	private ApplicationContext _context;

	public SettingsController(ApplicationContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> CreateUserSettings(UserSettings userSettings)
	{
		_context.UserSettings.Add(userSettings);
		try
		{
			await _context.SaveChangesAsync();
			return Created(userSettings.Id.ToString(), userSettings);
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> UpdateUserSettings(Guid id, UserSettings updatedSettings)
	{
		if (id != updatedSettings.Id)
		{
			return BadRequest();
		}
		
		var userSettings = await _context.UserSettings.FindAsync(id);
		if (userSettings is null)
		{
			return NotFound();
		}
		
		userSettings.PreferLightMode  = updatedSettings.PreferLightMode;
		userSettings.PreferMiniDrawer = updatedSettings.PreferMiniDrawer;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException) when (!_context.UserSettings.Any(x => x.Id == id))
		{
			return NotFound();
		}

		return NoContent();
	}
}