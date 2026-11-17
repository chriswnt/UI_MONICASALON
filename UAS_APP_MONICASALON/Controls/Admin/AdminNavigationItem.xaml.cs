using System.Windows.Input;

namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class AdminNavigationItem : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(AdminNavigationItem),
        string.Empty,
        propertyChanged: OnAccessibilityInputChanged);

    public static readonly BindableProperty IconGlyphProperty = BindableProperty.Create(
        nameof(IconGlyph),
        typeof(string),
        typeof(AdminNavigationItem),
        string.Empty);

    public static readonly BindableProperty RouteProperty = BindableProperty.Create(
        nameof(Route),
        typeof(string),
        typeof(AdminNavigationItem),
        string.Empty,
        propertyChanged: OnAccessibilityInputChanged);

    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
        nameof(IsActive),
        typeof(bool),
        typeof(AdminNavigationItem),
        false,
        propertyChanged: OnAccessibilityInputChanged);

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(AdminNavigationItem));

    /// <summary>
    /// Screen-reader label for the tab. Includes the active state and whether the
    /// tab already has a page, so assistive tech does not announce a dead end.
    /// </summary>
    public static readonly BindableProperty ScreenReaderTextProperty = BindableProperty.Create(
        nameof(ScreenReaderText),
        typeof(string),
        typeof(AdminNavigationItem),
        string.Empty);

    public AdminNavigationItem()
    {
        InitializeComponent();
        UpdateScreenReaderText();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public string Route
    {
        get => (string)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public string ScreenReaderText
    {
        get => (string)GetValue(ScreenReaderTextProperty);
        private set => SetValue(ScreenReaderTextProperty, value);
    }

    private static void OnAccessibilityInputChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((AdminNavigationItem)bindable).UpdateScreenReaderText();

    private void UpdateScreenReaderText()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ScreenReaderText = string.Empty;
            return;
        }

        if (string.IsNullOrWhiteSpace(Route))
        {
            ScreenReaderText = $"{Title}, halaman belum tersedia";
            return;
        }

        ScreenReaderText = IsActive ? $"{Title}, halaman aktif" : Title;
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (Command is null || !Command.CanExecute(Route))
        {
            return;
        }

        Command.Execute(Route);
    }
}
