using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Pages.Movements;

public partial class MovementForm
{
	[Parameter] public bool IsEdit { get; set; }
	[Parameter] public Movement? Movement { get; set; }
	[Parameter] public Guid? TrainerId { get; set; }
	
	[Inject] private HttpClient HttpClient { get; set; }
	[Inject] private ISnackbar Snackbar { get; set; }
	[Inject] private NavigationManager NavigationManager { get; set; }

	private string _tagsString;

	protected override async Task OnParametersSetAsync()
	{
		Movement ??= new Movement();
		if (TrainerId is not null) Movement.CreatedBy = TrainerId.Value;
		
		await base.OnParametersSetAsync();
	}

	private async Task Save()
	{
		if (!IsEdit)
		{
			Console.WriteLine($"App: Movement date {Movement.CreatedDateUTC}");
			Console.WriteLine($"App: Movement ID {Movement.Id}");
			Console.WriteLine($"App: Movement PMG {Movement.PrimaryMuscleGroup:G}");
			var result = await HttpClient.PostAsJsonAsync("api/Movements", Movement);

			if (result.IsSuccessStatusCode)
			{
				Snackbar.Add("Movement saved", Severity.Success);
				NavigationManager.NavigateTo("MovementLibrary");
			}
			else
			{
				Snackbar.Add("Error saving movement, try again", Severity.Error);
			}
		}
	}

	private void Cancel() => NavigationManager.NavigateTo("MovementLibrary");

	private void UpdateMovementTags(string tags)
	{
		Movement.Tags = tags.Split(',').Select(x => x.Trim()).ToList();
		_tagsString   = tags;
	}

	private void DeleteTag(MudChip chip)
	{
		Movement.Tags.Remove((string)chip.Value);
		_tagsString = string.Join(", ", Movement.Tags);
	}

	private void UpdateSecondaryMuscleGroup(MuscleGroup value) => Movement.SecondaryMuscleGroup = value;
}