using UraniumUI.Icons.MaterialSymbols;

namespace UAS_APP_MONICASALON.Icons;

/// <summary>
/// Compatibility aliases for the Material Symbols glyphs referenced by the PRD.
/// UraniumUI 3.0 (the .NET 10-compatible release) exposes the glyphs through
/// <see cref="MaterialOutlined"/>. The PRD uses the older "MaterialRegular" naming,
/// so this class keeps the XAML markup identical to the PRD.
/// </summary>
public static class MaterialRegular
{
    public const string Person = MaterialOutlined.Person;
    public const string Lock = MaterialOutlined.Lock;
    public const string Badge = MaterialOutlined.Badge;
    public const string Phone_iphone = MaterialOutlined.Phone_iphone;
    public const string Mail = MaterialOutlined.Mail;
    public const string Lock_reset = MaterialOutlined.Lock_reset;
    public const string Arrow_back = MaterialOutlined.Arrow_back;

    // "Location_on" was renamed to "Place" in current Material Symbols.
    public const string Location_on = MaterialOutlined.Place;
    public const string Notifications = MaterialOutlined.Notifications;
    public const string Search = MaterialOutlined.Search;
    public const string Tune = MaterialOutlined.Tune;

    public const string Content_cut = MaterialOutlined.Content_cut;
    public const string Spa = MaterialOutlined.Spa;
    public const string Auto_awesome = MaterialOutlined.Auto_awesome;
    public const string Palette = MaterialOutlined.Palette;
    public const string Face_retouching_natural = MaterialOutlined.Face_retouching_natural;
    public const string Straighten = MaterialOutlined.Straighten;

    public const string Favorite = MaterialOutlined.Favorite;
    public const string Star = MaterialOutlined.Star;

    public const string Home = MaterialOutlined.Home;
    public const string Explore = MaterialOutlined.Explore;
    public const string Calendar_month = MaterialOutlined.Calendar_month;
    public const string History = MaterialOutlined.History;

    // Outline variants are no longer separate glyph names in Material Symbols 3.x.
    public const string Chat_bubble_outline = MaterialOutlined.Chat_bubble;
    public const string Person_outline = MaterialOutlined.Person;

    public const string Close = MaterialOutlined.Close;
    public const string Expand_more = MaterialOutlined.Expand_more;
    public const string Check = MaterialOutlined.Check;
    public const string Check_circle = MaterialOutlined.Check_circle;
    public const string Schedule = MaterialOutlined.Schedule;
    public const string Notes = MaterialOutlined.Notes;
    public const string Info = MaterialOutlined.Info;
    public const string Warning = MaterialOutlined.Warning;
    public const string Chevron_right = MaterialOutlined.Chevron_right;
    public const string Arrow_forward = MaterialOutlined.Arrow_forward;

    // Booking, history, and feedback surfaces.
    public const string Event = MaterialOutlined.Event;
    public const string Payments = MaterialOutlined.Payments;
    public const string Edit = MaterialOutlined.Edit;
    public const string Cancel = MaterialOutlined.Cancel;
    public const string Verified = MaterialOutlined.Verified;
    public const string Task_alt = MaterialOutlined.Task_alt;
}
