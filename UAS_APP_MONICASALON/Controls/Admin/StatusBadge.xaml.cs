using UAS_APP_MONICASALON.Models.Admin;

namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class StatusBadge : ContentView
{
    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        nameof(Status),
        typeof(BookingStatus),
        typeof(StatusBadge),
        BookingStatus.Pending);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(StatusBadge),
        string.Empty);

    public StatusBadge()
    {
        InitializeComponent();
    }

    public BookingStatus Status
    {
        get => (BookingStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
