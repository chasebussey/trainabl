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
}