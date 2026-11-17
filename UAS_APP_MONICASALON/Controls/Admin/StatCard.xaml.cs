namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class StatCard : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(StatCard),
        string.Empty);

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(string),
        typeof(StatCard),
        string.Empty);

    public static readonly BindableProperty CaptionProperty = BindableProperty.Create(
        nameof(Caption),
        typeof(string),
        typeof(StatCard),
        string.Empty);

    public static readonly BindableProperty IconGlyphProperty = BindableProperty.Create(
        nameof(IconGlyph),
        typeof(string),
        typeof(StatCard),
        string.Empty);

    public StatCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }
}
