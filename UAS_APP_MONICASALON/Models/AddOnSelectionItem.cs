using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models;

public partial class AddOnSelectionItem : ObservableObject
{
    public AddOnSelectionItem(SalonAddOn addOn, bool isSelected = false)
    {
        AddOn = addOn;
        IsSelected = isSelected;
    }

    public SalonAddOn AddOn { get; }

    public event EventHandler? SelectionChanged;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    partial void OnIsSelectedChanged(bool value)
    {
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}
