using System.Windows.Input;

namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class SectionCard : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(SectionCard),
        string.Empty);

    public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
        nameof(Subtitle),
        typeof(string),
        typeof(SectionCard),
        string.Empty,
        propertyChanged: OnSubtitleChanged);

    public static readonly BindableProperty ActionTextProperty = BindableProperty.Create(
        nameof(ActionText),
        typeof(string),
        typeof(SectionCard),
        string.Empty,
        propertyChanged: OnActionTextChanged);

    public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(
        nameof(ActionCommand),
        typeof(ICommand),
        typeof(SectionCard));

    public static readonly BindableProperty CardContentProperty = BindableProperty.Create(
        nameof(CardContent),
        typeof(View),
        typeof(SectionCard),
        propertyChanged: OnCardContentChanged);

    public SectionCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string ActionText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public View? CardContent
    {
        get => (View?)GetValue(CardContentProperty);
        set => SetValue(CardContentProperty, value);
    }

    public bool HasSubtitle => !string.IsNullOrWhiteSpace(Subtitle);

    public bool HasAction => !string.IsNullOrWhiteSpace(ActionText);

    private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((SectionCard)bindable).OnPropertyChanged(nameof(HasSubtitle));

    private static void OnActionTextChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((SectionCard)bindable).OnPropertyChanged(nameof(HasAction));

    private static void OnCardContentChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((SectionCard)bindable).UpdateCardContent();

    private void UpdateCardContent()
    {
        if (SectionContentHost is null)
        {
            return;
        }

        SectionContentHost.Content = CardContent;
    }
}
