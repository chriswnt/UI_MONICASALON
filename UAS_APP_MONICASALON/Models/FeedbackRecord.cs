using System.Globalization;

namespace UAS_APP_MONICASALON.Models;

public class FeedbackRecord
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");

    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string BookingId { get; init; } = string.Empty;

    public int Rating { get; init; }

    public List<string> Tags { get; init; } = new();

    public string Comment { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.Now;

    public string RatingText => Rating switch
    {
        5 => "Luar biasa",
        4 => "Sangat baik",
        3 => "Baik",
        2 => "Cukup",
        1 => "Kurang",
        _ => "Belum dinilai"
    };

    public string StarsText => new string('★', Math.Clamp(Rating, 0, 5)) +
                              new string('☆', Math.Max(0, 5 - Rating));

    public string TagsText => Tags.Count == 0 ? "Tanpa tag" : string.Join(" • ", Tags);

    public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

    public string CommentText => HasComment ? Comment : "Tidak ada catatan tambahan.";

    public string CreatedText => CreatedAt.ToString("d MMMM yyyy", Indonesian);
}
