using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Trainabl.Shared.Models;

namespace Trainabl.Server.Services;

public class AccessControlService
{
	private ApplicationContext _context;

	public AccessControlService(ApplicationContext context)
	{
		_context = context;
	}
	
	
	public async Task<bool> IsAuthorizedForClient(ClaimsPrincipal user, ClientProfile client)
	{
		var email  = user.Claims.First(x => x.Type == "https://app.trainabl.co/claims/email").Value;
		
		if (user.IsInRole("Trainer"))
		{
			var trainer = await _context.TrainerProfiles.FirstOrDefaultAsync(x => x.Email == email);
			if (trainer == null || trainer.Id != client.TrainerProfileId)
			{
				return false;
			}
		}
		else if (user.IsInRole("Client"))
		{
			var clientUser = await _context.ClientProfiles.FirstOrDefaultAsync(x => x.Email == email);
			if (clientUser == null || clientUser.Id != client.Id)
			{
				return false;
			}
		}

		return true;
	}

	public async Task<bool> IsAuthorizedForTrainer(ClaimsPrincipal user, TrainerProfile trainer)
	{
		var email = user.Claims.First(x => x.Type == "https://app.trainabl.co/claims/email").Value;

		if (user.IsInRole("Trainer"))
		{
			// the user is not the trainer accessed
			if (email != trainer.Email)
			{
				return false;
			}
		}
		else if (user.IsInRole("Client"))
		{
			var client = await _context.ClientProfiles.FirstOrDefaultAsync(x => x.Email == email);
			if (client == null || client.TrainerProfileId != trainer.Id)
			{
				return false;
			}
		}

		return true;
	}

	public async Task<bool> IsAuthorizedForWorkout(ClaimsPrincipal user, Workout workout)
	{
		var email = user.Claims.First(x => x.Type == "https://app.trainabl.co/claims/email").Value;

		if (user.IsInRole("Trainer"))
		{
			var trainer = await _context.TrainerProfiles.FirstOrDefaultAsync(x => x.Email == email);

			if (trainer == null || workout.TrainerProfileId != trainer.Id) return false;
		}
		else if (user.IsInRole("Client"))
		{
			var client = await _context.ClientProfiles.FirstOrDefaultAsync(x => x.Email == email);

			if (client == null || workout.ClientProfileId != client.Id) return false;
		}

		return true;
	}
}