using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainabl.Server.Services;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
	private ApplicationContext _context;
	private AccessControlService _accessControl;

	public MetricsController(ApplicationContext context, AccessControlService accessControl)
	{
		_context       = context;
		_accessControl = accessControl;
	}

	[HttpPost]
	public async Task<IActionResult> CreateMetric(Metric metric)
	{
		var user   = HttpContext.User;
		var client = await _context.ClientProfiles.FindAsync(metric.ClientProfileId);

		if (client is null) return NotFound();

		var isAuthorizedForClient = await _accessControl.IsAuthorizedForClient(user, client);
		if (!isAuthorizedForClient) return Forbid();
		
		metric.Id = Guid.NewGuid();

		_context.Metrics.Add(metric);

		try
		{
			await _context.SaveChangesAsync();
			return Created(metric.Id.ToString(), metric);
		}
		catch (Exception e)
		{
			return UnprocessableEntity();
		}
	}
}