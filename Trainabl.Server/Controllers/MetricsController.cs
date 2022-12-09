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

		if (metric.Name.Equals("Weight", StringComparison.OrdinalIgnoreCase) ||
		    metric.Name.Equals("Height", StringComparison.OrdinalIgnoreCase))
		{
			var bmi = metric.Name switch
			{
				"Weight" => CalculateBMI(
					metric, client.Metrics.Where(x => x.Name == "Height").OrderByDescending(x => x.CreatedUTC).First()),
				"Height" => CalculateBMI(
					client.Metrics.Where(x => x.Name == "Weight").OrderByDescending(x => x.CreatedUTC).First(), metric)
			};
			bmi.ClientProfileId = client.Id;
			_context.Metrics.Add(bmi);
		}

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

	#region Helpers

	private Metric CalculateBMI(Metric weight, Metric height)
	{
		var bmi = new Metric
		{
			Name       = "BMI",
			CreatedUTC = DateTime.UtcNow,
			Id         = Guid.NewGuid()
		};
		
		bmi.Value = weight.Value / (height.Value * height.Value) * 703;
		return bmi;
	}

	#endregion
}