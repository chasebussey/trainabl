using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Pages;

public partial class WorkoutForm
{
	[Parameter] public bool IsEdit { get; set; }

	[Parameter] public WorkoutDTO? Workout { get; set; }

	[Inject] private HttpClient HttpClient { get; set; }
	[Inject] private ISnackbar Snackbar { get; set; }
	[Inject] ILocalStorageService LocalStorageService { get; set; }

	private Guid _trainerId;
	private int _circuitNumExercises;
	private string _circuitTime;

	protected override async Task OnParametersSetAsync()
	{
		Workout ??= new WorkoutDTO()
		{
			Exercises = new List<Exercise>
			{
				new()
				{
					MovementName = ""
				}
			}
		};

		var trainerProfile = await LocalStorageService.GetItemAsync<TrainerProfile>("trainerProfile");
		_trainerId = trainerProfile.Id;

		Workout.TrainerProfileId = _trainerId;
		
		await base.OnParametersSetAsync();
	}

	private void AddExercise()
	{
		Workout.Exercises.Add(
			new Exercise
			{
				MovementName = ""
			}
		);
	}

	private void RemoveExercise(Exercise ex)
	{
		Workout.Exercises.Remove(ex);
	}

	private void UpdateCircuitExercises(int num){ }
	private void UpdateCircuitTime(string val) { }

	private async Task Save(bool isDraft = false)
	{
		if (!isDraft)
		{
			Workout.IsDraft = false;
		}

		if (!IsEdit)
		{
			var result = await HttpClient.PostAsJsonAsync("api/workouts", Workout);
			if (result.IsSuccessStatusCode)
			{
				Snackbar.Add("Workout saved!", Severity.Success);
			}
			else
			{
				Snackbar.Add("Error saving workout, try again.", Severity.Error);
			}
		}
	}
}