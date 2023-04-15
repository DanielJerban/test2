using SkiaSharp;

namespace BPMS.Application.Captcha.Models;

public class CaptchaGenerator : IDisposable
{
    private readonly CaptchaOptions options;

    public CaptchaGenerator(CaptchaOptions options) => this.options = options;

    public byte[] GenerateImageAsByteArray(string captchaCode, int? width = null, int? height = null)
        => BuildImage(captchaCode, width ?? options.Width, height ?? options.Height).Encode(options.EncoderType, options.ImageQuality).ToArray();

    public Stream GenerateImageAsStream(string captchaCode, int? width = null, int? height = null)
        => BuildImage(captchaCode, width ?? options.Width, height ?? options.Height).Encode(options.EncoderType, options.ImageQuality).AsStream();

    public static float GenerateNextFloat(double min = -3.40282347E+38, double max = 3.40282347E+38)
    {
        Random random = new Random();
        double range = max - min;
        double sample = random.NextDouble();
        double scaled = sample * range + min;
        float result = (float)scaled;
        return result;
    }

    protected SKSurface DrawText(SKSurface plainSkSurface, string stringText, int width, int height)
    {
        if (string.IsNullOrEmpty(stringText))
            throw new ArgumentException($@"'{nameof(stringText)}' cannot be null or empty.", nameof(stringText));

        if (stringText.Length == 1)
            throw new ArgumentException($@"'{nameof(stringText)}' length must be greater than one charachter.", nameof(stringText));

        var plainCanvas = plainSkSurface.Canvas;
        plainCanvas.Clear(options.BackgroundColor);

        using (var paintInfo = new SKPaint())
        {
            Random random = new Random();
            string fontName = null;
            if (options.FontFamilies.Length > 0)
                fontName = options.FontFamilies[random.Next(0, options.FontFamilies.Length)];

            float? stringLength = null;
            int xToDraw = 0;
            int yToDraw = 0;
            foreach (var c in stringText)
            {
                var text = c.ToString();

                paintInfo.Typeface = SKTypeface.FromFamilyName(fontName, SKFontStyle.Italic);
                paintInfo.TextSize = options.FontSize;
                paintInfo.Color = options.TextColor[random.Next(0, options.TextColor.Length)];
                paintInfo.IsAntialias = true;

                if (stringLength == null)
                {
                    stringLength = paintInfo.MeasureText(stringText);
                    if (stringLength > width)
                        throw new ArgumentException($@"'{nameof(width)}' must be greater than {stringLength}.", nameof(width));

                    xToDraw = (width - (int)stringLength.Value) / 2;
                    yToDraw = (height - options.FontSize) / 2 + options.FontSize;
                }

                plainCanvas.DrawText(text, xToDraw, yToDraw, paintInfo);

                var charLength = paintInfo.MeasureText(text);
                xToDraw += (int)charLength;
            }
        }
        plainCanvas.Flush();

        return plainSkSurface;
    }

    protected SKSurface DrawLines(SKSurface plainSkSurface, int width, int height)
    {
        var captchaCanvas = plainSkSurface.Canvas;

        using (var paintInfo = new SKPaint())
        {
            Random random = new Random();
            var center = width / 2;
            var middle = height / 2;

            Parallel.For(0, options.DrawLines, i =>
            {
                int x0 = random.Next(0, center);
                int y0 = random.Next(0, middle);
                int x1 = random.Next(center, width);
                int y1 = random.Next(middle, height);
                var color = options.DrawLinesColor[random.Next(0, options.DrawLinesColor.Length)];
                var thickness = GenerateNextFloat(options.MinLineThickness, options.MaxLineThickness);

                captchaCanvas.DrawLine(x0, y0, x1, y1, new SKPaint()
                {
                    Color = color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = thickness
                });
            });
        }
        captchaCanvas.Flush();

        return plainSkSurface;
    }

    protected SKSurface DrawNoises(SKSurface plainSkSurface, int width, int height)
    {
        var captchaCanvas = plainSkSurface.Canvas;

        using (var paintInfo = new SKPaint())
        {
            Random random = new Random();

            Parallel.For(0, options.NoiseRate, i =>
            {
                int x0 = random.Next(0, width);
                int y0 = random.Next(0, height);
                var color = options.NoiseRateColor[random.Next(0, options.NoiseRateColor.Length)];
                var thickness = GenerateNextFloat(0.5, 1.5);

                captchaCanvas.DrawPoint(x0, y0, new SKPaint()
                {
                    Color = color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = thickness
                });
            });
        }
        captchaCanvas.Flush();

        return plainSkSurface;
    }

    protected SKImage BuildImage(string captchaCode, int width, int height)
    {
        var imageInfo = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

        using (var captchaSkSurface = SKSurface.Create(imageInfo))
        {
            _ = DrawText(captchaSkSurface, captchaCode, width, height);

            if (options.DrawLines > 0)
                _ = DrawLines(captchaSkSurface, width, height);

            if (options.NoiseRate > 0)
                _ = DrawNoises(captchaSkSurface, width, height);

            return captchaSkSurface.Snapshot();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}