using UAS_APP_MONICASALON.Models.Admin;

namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class PaymentBadge : ContentView
{
    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        nameof(Status),
        typeof(PaymentStatus),
        typeof(PaymentBadge),
        PaymentStatus.Unpaid);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(PaymentBadge),
        string.Empty);

    public PaymentBadge()
    {
        InitializeComponent();
    }

    public PaymentStatus Status
    {
        get => (PaymentStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
