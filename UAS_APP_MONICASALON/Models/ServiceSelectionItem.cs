using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models;

public partial class ServiceSelectionItem : ObservableObject
{
    public ServiceSelectionItem(ServiceItem service, bool isSelected = false)
    {
        Service = service;
        IsSelected = isSelected;
    }

    public ServiceItem Service { get; }

    public event EventHandler? SelectionChanged;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    partial void OnIsSelectedChanged(bool value)
    {
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}
