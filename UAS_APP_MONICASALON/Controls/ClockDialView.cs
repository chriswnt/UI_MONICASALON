using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Controls;

public enum ClockDialMode
{
    Hour,
    Minute
}

/// <summary>
/// Material-style clock face used by the customer time picker. Tapping the
/// dial chooses an hour (1-12) or a minute (0-59); the owning view model maps
/// the 12-hour value to a 24-hour <see cref="TimeSpan"/>.
/// </summary>
public class ClockDialView : ContentView
{
    public static readonly BindableProperty ModeProperty = BindableProperty.Create(
        nameof(Mode),
        typeof(ClockDialMode),
        typeof(ClockDialView),
        ClockDialMode.Hour,
        propertyChanged: OnDialPropertyChanged);

    public static readonly BindableProperty HourProperty = BindableProperty.Create(
        nameof(Hour),
        typeof(int),
        typeof(ClockDialView),
        12,
        BindingMode.TwoWay,
        propertyChanged: OnDialPropertyChanged);

    public static readonly BindableProperty MinuteProperty = BindableProperty.Create(
        nameof(Minute),
        typeof(int),
        typeof(ClockDialView),
        0,
        BindingMode.TwoWay,
        propertyChanged: OnDialPropertyChanged);

    private readonly GraphicsView _graphicsView;

    public ClockDialView()
    {
        _graphicsView = new GraphicsView
        {
            Drawable = new ClockDialDrawable(this)
        };

        _graphicsView.StartInteraction += OnStartInteraction;
        _graphicsView.DragInteraction += OnDragInteraction;

        Content = _graphicsView;
    }

    public ClockDialMode Mode
    {
        get => (ClockDialMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public int Hour
    {
        get => (int)GetValue(HourProperty);
        set => SetValue(HourProperty, value);
    }

    public int Minute
    {
        get => (int)GetValue(MinuteProperty);
        set => SetValue(MinuteProperty, value);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _graphicsView.Invalidate();
    }

    private static void OnDialPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ClockDialView)bindable)._graphicsView.Invalidate();
    }

    private void OnStartInteraction(object? sender, TouchEventArgs e) => SelectTouch(e);

    private void OnDragInteraction(object? sender, TouchEventArgs e) => SelectTouch(e);

    private void SelectTouch(TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
        {
            return;
        }

        var point = e.Touches[0];
        var width = _graphicsView.Width;
        var height = _graphicsView.Height;
        var size = Math.Min(width, height);

        if (size <= 0)
        {
            return;
        }

        var centerX = width / 2d;
        var centerY = height / 2d;
        var dx = point.X - centerX;
        var dy = point.Y - centerY;
        var distance = Math.Sqrt((dx * dx) + (dy * dy));

        if (distance < 16 || distance > (size / 2d))
        {
            return;
        }

        var angle = Math.Atan2(dy, dx) * 180d / Math.PI;
        var clockAngle = (angle + 90d + 360d) % 360d;

        if (Mode == ClockDialMode.Hour)
        {
            var index = (int)Math.Round(clockAngle / 30d, MidpointRounding.AwayFromZero) % 12;
            Hour = index == 0 ? 12 : index;
            return;
        }

        Minute = (int)Math.Round(clockAngle / 6d, MidpointRounding.AwayFromZero) % 60;
    }

    private sealed class ClockDialDrawable : IDrawable
    {
        private static readonly Color DialColor = Color.FromArgb("#F5F0EE");
        private static readonly Color NumberColor = Color.FromArgb("#4A3A38");
        private static readonly Color AccentColor = Color.FromArgb("#D9534F");
        private static readonly Color SelectedTextColor = Colors.White;

        private readonly ClockDialView _owner;

        public ClockDialDrawable(ClockDialView owner) => _owner = owner;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var width = dirtyRect.Width;
            var height = dirtyRect.Height;
            var size = Math.Min(width, height);

            if (size <= 0)
            {
                return;
            }

            canvas.Antialias = true;

            var center = new PointF(dirtyRect.Center.X, dirtyRect.Center.Y);
            var outerRadius = (size / 2f) - 4f;
            var numberRadius = outerRadius - Math.Max(28f, size * 0.13f);
            var fontSize = Math.Max(12f, size * 0.052f);
            var bubbleRadius = Math.Max(17f, fontSize * 1.25f);

            canvas.FillColor = DialColor;
            canvas.FillCircle(center.X, center.Y, outerRadius);

            var selectedPoint = GetSelectionPoint(center, numberRadius);

            if (selectedPoint.HasValue)
            {
                canvas.StrokeColor = AccentColor;
                canvas.StrokeSize = 2f;
                canvas.DrawLine(
                    center.X,
                    center.Y,
                    selectedPoint.Value.X,
                    selectedPoint.Value.Y);
            }

            if (_owner.Mode == ClockDialMode.Hour)
            {
                DrawHourLabels(canvas, center, numberRadius, fontSize);
            }
            else
            {
                DrawMinuteLabels(canvas, center, numberRadius, fontSize);
            }

            if (selectedPoint.HasValue)
            {
                canvas.FillColor = AccentColor;
                canvas.FillCircle(selectedPoint.Value.X, selectedPoint.Value.Y, bubbleRadius);
                canvas.FillCircle(center.X, center.Y, 4.5f);

                var selectedText = _owner.Mode == ClockDialMode.Hour
                    ? _owner.Hour.ToString()
                    : _owner.Minute.ToString("00");

                DrawLabel(canvas, selectedText, selectedPoint.Value, fontSize, SelectedTextColor);
            }
        }

        private void DrawHourLabels(ICanvas canvas, PointF center, float radius, float fontSize)
        {
            for (var index = 0; index < 12; index++)
            {
                var hour = index == 0 ? 12 : index;

                if (hour == _owner.Hour)
                {
                    continue;
                }

                var point = PointOnDial(center, radius, index * 30f);
                DrawLabel(canvas, hour.ToString(), point, fontSize, NumberColor);
            }
        }

        private void DrawMinuteLabels(ICanvas canvas, PointF center, float radius, float fontSize)
        {
            for (var index = 0; index < 12; index++)
            {
                var minute = index * 5;

                if (minute == _owner.Minute)
                {
                    continue;
                }

                var point = PointOnDial(center, radius, index * 30f);
                DrawLabel(canvas, minute.ToString("00"), point, fontSize, NumberColor);
            }
        }

        private PointF? GetSelectionPoint(PointF center, float radius)
        {
            var angle = _owner.Mode == ClockDialMode.Hour
                ? (_owner.Hour == 12 ? 0f : _owner.Hour * 30f)
                : _owner.Minute * 6f;

            return PointOnDial(center, radius, angle);
        }

        private static void DrawLabel(ICanvas canvas, string text, PointF center, float fontSize, Color color)
        {
            var halfSize = fontSize * 1.8f;

            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.FontSize = fontSize;
            canvas.FontColor = color;
            canvas.DrawString(
                text,
                center.X - halfSize,
                center.Y - halfSize,
                halfSize * 2f,
                halfSize * 2f,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        private static PointF PointOnDial(PointF center, float radius, float angleDegrees)
        {
            var radians = (angleDegrees - 90f) * Math.PI / 180f;

            return new PointF(
                center.X + (radius * (float)Math.Cos(radians)),
                center.Y + (radius * (float)Math.Sin(radians)));
        }
    }
}
