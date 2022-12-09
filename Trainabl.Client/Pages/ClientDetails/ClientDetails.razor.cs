using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Pages.ClientDetails;

public partial class ClientDetails
{
	[Parameter]
	public Guid ClientId { get; set; }
	
	[Inject] private HttpClient HttpClient { get; set; }
	[Inject] private ILocalStorageService LocalStorageService { get; set; }
	[Inject] private ISnackbar Snackbar { get; set; }
	
	private bool _initialized;
	private Guid _trainerId;
	private ClientProfileDTO _client;
	private Guid _clientId;
	
	protected override async Task OnInitializedAsync()
	{
		var trainerProfile = await LocalStorageService.GetItemAsync<TrainerProfile>("trainerProfile");
		_trainerId = trainerProfile.Id;
		await LoadClient();
		_initialized = true;
		await base.OnInitializedAsync();
	}

	private async Task LoadClient()
	{
		try
		{
			_client = await HttpClient.GetFromJsonAsync<ClientProfileDTO>($"api/Trainers/{_trainerId}/clients/{ClientId}");
		}
		catch (Exception e)
		{
			Snackbar.Add("Error loading client, please refresh", Severity.Error);
		}
	}
}