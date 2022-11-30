using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Pages.Workouts;

public partial class WorkoutForm
{
	[Parameter] public bool IsEdit { get; set; }

	[Parameter] public WorkoutDTO? Workout { get; set; }
	[Parameter] public Guid? ClientId { get; set; }

	[Inject] private HttpClient HttpClient { get; set; }
	[Inject] private ISnackbar Snackbar { get; set; }
	[Inject] ILocalStorageService LocalStorageService { get; set; }
	[Inject] private IJSRuntime JsRuntime { get; set; }

	private Guid _trainerId;
	private int _circuitNumExercises;
	private string _circuitTime;
	private MudDataGrid<Exercise> _dataGrid;
	private MudTextField<string> _lastMovementName;
	private bool _newExerciseRowExists;

	protected override async Task OnParametersSetAsync()
	{
		Workout ??= new WorkoutDTO()
		{
			Exercises = new List<Exercise>
			{
				new()
				{
					MovementName = "",
					Index = 0
				}
			}
		};

		var trainerProfile = await LocalStorageService.GetItemAsync<TrainerProfile>("trainerProfile");
		_trainerId = trainerProfile.Id;

		Workout.TrainerProfileId = _trainerId;
		
		await base.OnParametersSetAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			if (_newExerciseRowExists)
			{
				await JsRuntime.InvokeVoidAsync("FocusLastRow");
				_newExerciseRowExists = false;
			}
		}
		
		await base.OnAfterRenderAsync(firstRender);
	}

	private void AddExercise()
	{
		var lastIndex = 0;
		
		if (Workout!.Exercises.Count > 0)
		{
			lastIndex = Workout.Exercises.MaxBy(x => x.Index)!.Index;
		}
		Workout.Exercises.Add(
			new Exercise
			{
				MovementName = "",
				Index = lastIndex + 1
			}
		);
	}

	private void RemoveExercise(Exercise ex)
	{
		Workout!.Exercises.Remove(ex);
	}

	private void UpdateCircuitExercises(int num){ }
	private void UpdateCircuitTime(string val) { }

	private async Task Save(bool isDraft = false)
	{
		Workout!.IsDraft = isDraft;

		if (ClientId is not null && ClientId != Guid.Empty)
		{
			Workout.ClientProfileId = ClientId;
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
		else
		{
			var result = await HttpClient.PutAsJsonAsync($"api/workouts/{Workout.Id}", Workout);
			if (result.IsSuccessStatusCode)
			{
				Snackbar.Add("Workout saved.", Severity.Success);
			}
			else
			{
				Snackbar.Add("Error saving workout, try again.", Severity.Error);
			}
		}
	}

	private async Task HandleKeyDown(KeyboardEventArgs e)
	{
		if (e.Key == "Enter")
		{
			_newExerciseRowExists = true;
			AddExercise();
		}
		
	}

	private void MoveExerciseUp(Exercise ex)
	{
		var targetIndex = ex.Index - 1;

		if (Workout.Exercises.Any(x => x.Index == targetIndex))
		{
			var swapEx = Workout.Exercises.First(x => x.Index == targetIndex);
			swapEx.Index = ex.Index;
		}

		ex.Index          = targetIndex;
		Workout.Exercises = Workout.Exercises.OrderBy(x => x.Index).ToList();
	}

	private void MoveExerciseDown(Exercise ex)
	{
		var targetIndex = ex.Index + 1;

		if (Workout.Exercises.Any(x => x.Index == targetIndex))
		{
			var swapEx = Workout.Exercises.First(x => x.Index == targetIndex);
			swapEx.Index = ex.Index;
		}

		ex.Index          = targetIndex;
		Workout.Exercises = Workout.Exercises.OrderBy(x => x.Index).ToList();
	}
}