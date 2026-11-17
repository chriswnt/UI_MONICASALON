using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Drawables;

/// <summary>
/// Draws the white wave that joins the coral login hero to the form sheet.
/// Drawing it instead of using a clipped Shape keeps the left/right edges
/// fully covered at every window width.
/// </summary>
public sealed class LoginWaveDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var path = new PathF();

        // Start slightly outside the canvas so anti-aliasing never leaves the
        // hero image visible in the first/last pixel column.
        path.MoveTo(-1, height * 0.60f);
        path.CurveTo(
            width * 0.13f, height * 0.92f,
            width * 0.31f, height * 0.14f,
            width * 0.50f, height * 0.33f);
        path.CurveTo(
            width * 0.70f, height * 0.52f,
            width * 0.82f, height * 0.88f,
            width + 1, height * 0.46f);
        path.LineTo(width + 1, height + 1);
        path.LineTo(-1, height + 1);
        path.Close();

        canvas.FillColor = Colors.White;
        canvas.FillPath(path);
    }
}
