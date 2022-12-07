using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Trainabl.Shared.Models;

namespace Trainabl.Client.Pages.WorkoutNotes;

public partial class WorkoutNotesGrid
{
    [CascadingParameter(Name = "ClientId")] public Guid? ClientId { get; set; }
    [CascadingParameter(Name = "TrainerId")] public Guid TrainerId { get; set; }

    private MudDateRangePicker _picker;
    private MudTable<DisplayNote> _table;
    private Guid _trainerId;
    private List<DisplayNote> _pagedData;
    private List<WorkoutDTO> _workouts;
    private List<ClientProfile> _clients;
    private DateRange? _dateRange;
    private bool _initialized;
    private string _searchString;

    protected override async Task OnParametersSetAsync()
    {
        var uri = ClientId is null ? $"api/trainers/{TrainerId}/workouts" : $"api/clients/{ClientId}/workouts";

        try
        {
            _workouts = await HttpClient.GetFromJsonAsync<List<WorkoutDTO>>(uri);
        }
        catch (Exception e)
        {
        }

        try
        {
            _clients = await HttpClient.GetFromJsonAsync<List<ClientProfile>>($"api/Trainers/{TrainerId}/clients");
        }
        catch (Exception e) { }
        
        _initialized = _workouts is not null && _workouts.Count > 0;
        await base.OnParametersSetAsync();
    }

    private async Task<TableData<DisplayNote>> ServerReload(TableState state)
    {
        List<WorkoutNote> data = new List<WorkoutNote>();
        
        var args = new Dictionary<string, string>();

        if (_dateRange != null)
        {
            DateTime start = _dateRange.Start.Value;
            DateTime end = _dateRange.End.Value;

            start = DateTime.SpecifyKind(start, DateTimeKind.Local);
            end = DateTime.SpecifyKind(end, DateTimeKind.Local);
            
            args.Add("startDate", start.ToUniversalTime().ToString());
            args.Add("endDate", end.ToUniversalTime().ToString());
        }

        if (!string.IsNullOrEmpty(_searchString))
        {
            args.Add("searchString", _searchString);
        }

        var uri = ClientId is null
            ? $"api/Trainers/{TrainerId}/workoutNotes/search"
            : $"api/Clients/{ClientId}/workoutNotes/search";
        uri = QueryHelpers.AddQueryString(uri, args);

        data = await HttpClient.GetFromJsonAsync<List<WorkoutNote>>(uri);

        List<DisplayNote> displayNotes = GenerateDisplayNotes(data);

        _pagedData = displayNotes.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();

        return new TableData<DisplayNote> { TotalItems = data.Count, Items = _pagedData };
    }

    private void OnDateRangeChanged(DateRange dateRange)
    {
        _dateRange = dateRange;
        _table.ReloadServerData();
    }

    private void OnSearch(string value)
    {
        _searchString = value;
        _table.ReloadServerData();
    }

    private List<DisplayNote> GenerateDisplayNotes(List<WorkoutNote> notes)
    {
        return notes.Select(note => new DisplayNote
        {
            Note = note,
            Workout = _workouts.FirstOrDefault(x => x.Id == note.WorkoutId),
            ClientName = _clients.FirstOrDefault(x => x.Id == note.ClientProfileId).Name,
            ShowNote = false
        }).ToList();
    }
}

internal class DisplayNote
{
    public WorkoutNote Note { get; set; }
    public WorkoutDTO Workout { get; set; }
    public string ClientName { get; set; }
    public bool ShowNote { get; set; }
}