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

	private string TagsString
	{
		get
		{
			if (Movement?.Tags != null && Movement.Tags.Count > 0)
			{
				return string.Join(", ", Movement.Tags);
			}

			return "";
		}
		set
		{
			if (Movement != null) Movement.Tags = value.Split(',').Select(x => x.Trim()).ToList();
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		Movement ??= new Movement();
		if (TrainerId is not null && !IsEdit) Movement.CreatedBy = TrainerId.Value;
		if (TrainerId is not null) Movement.LastModifiedBy       = TrainerId.Value;

		await base.OnParametersSetAsync();
	}

	private async Task Save()
	{
		if (!IsEdit)
		{
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
		else
		{
			var result = await HttpClient.PutAsJsonAsync($"api/Movements/{Movement.Id}", Movement);

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

	private void DeleteTag(MudChip chip)
	{
		Movement.Tags.Remove((string)chip.Value);
	}

	private void UpdateSecondaryMuscleGroup(MuscleGroup value) => Movement.SecondaryMuscleGroup = value;
}