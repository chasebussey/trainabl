using System.ComponentModel;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Services;

public class UserSettingsService
{
	private const string KeyName = "userSettings";

	private bool _initialized;
	private UserSettings _userSettings;
	private ILocalStorageService _localStorageService;
	private HttpClient _httpClient;
	private IJSRuntime _js;

	public event EventHandler Changed;

	public bool AutoSave { get; set; } = true;

	public UserSettingsService(ILocalStorageService localStorageService, HttpClient httpClient, IJSRuntime jsRuntime)
	{
		_localStorageService = localStorageService;
		_httpClient          = httpClient;
		_js                  = jsRuntime;
	}

	public async ValueTask<UserSettings> Get(bool fetchFromServer = false)
	{
		if (_userSettings is not null) return _userSettings;
		
		// Check for a user ID
		TrainerProfile? trainerProfile = await _localStorageService.GetItemAsync<TrainerProfile>("trainerProfile");
		Guid? trainerId      = trainerProfile?.Id;
		Console.WriteLine($"App: trainerId = {trainerId}");
		Console.WriteLine($"App: trainerId.Value = {trainerId.Value}");

		var result = await _localStorageService.GetItemAsync<UserSettings>(KeyName) ?? new UserSettings();
		Console.WriteLine($"App: result.Id = {result.Id}");
		if (trainerId is not null && result.UserId != trainerId.Value)
		{
			result.UserId = trainerId.Value;
		}

		result.PropertyChanged += OnPropertyChanged;
		_userSettings          =  result;
		return result;
	}

	public async Task Save()
	{
		Console.WriteLine("App: saving settings now");
		await _localStorageService.SetItemAsync(KeyName, _userSettings);
	}

	private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (AutoSave)
		{
			await Save();
		}
	}
}