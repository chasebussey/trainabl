using Microsoft.AspNetCore.Mvc;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
	private ApplicationContext _context;

	public MetricsController(ApplicationContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> CreateMetric(Metric metric)
	{
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