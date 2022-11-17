using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trainabl.Shared.Models;

public class UserSettings : INotifyPropertyChanged
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }

	public bool PreferMiniDrawer
	{
		get => _preferMiniDrawer;
		set
		{
			if (value == _preferMiniDrawer) return;
			_preferMiniDrawer = value; 
			OnPropertyChanged();
		}
	}
	public bool PreferLightMode
	{
		get => _preferLightMode;
		set
		{
			if (value == _preferLightMode) return;
			_preferLightMode = value;
			OnPropertyChanged();
		}
	}

	private bool _preferMiniDrawer;
	private bool _preferLightMode;

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}