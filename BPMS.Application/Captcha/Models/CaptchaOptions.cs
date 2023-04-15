using SkiaSharp;

namespace BPMS.Application.Captcha.Models;

public class CaptchaOptions
{
    public ushort Width { get; set; } = 180;
    public ushort Height { get; set; } = 50;
    public int ImageQuality { get; set; } = 80;
    public SKEncodedImageFormat EncoderType { get; set; } = SKEncodedImageFormat.Png;

    public string[] FontFamilies { get; set; } = new string[] { };
    public byte FontSize { get; set; } = 29;
    public SKFontStyle FontStyle { get; set; } = SKFontStyle.Italic;
    public SKColor BackgroundColor { get; set; } = SkDefault4BitColors.White;
    public SKColor[] TextColor { get; set; } = new SKColor[] { SkDefault4BitColors.Blue, SkDefault4BitColors.Black, SKColor.Parse("#A52A2A")/*Brown*/, SkDefault4BitColors.Grey, SkDefault4BitColors.Green };

    public byte DrawLines { get; set; } = 5;
    public SKColor[] DrawLinesColor { get; set; } = new SKColor[] { SkDefault4BitColors.Blue, SkDefault4BitColors.Black, SKColor.Parse("#A52A2A")/*Brown*/, SkDefault4BitColors.Grey, SkDefault4BitColors.Green };
    public float MinLineThickness { get; set; } = 0.7f;
    public float MaxLineThickness { get; set; } = 2.0f;

    public ushort NoiseRate { get; set; } = 800;
    public SKColor[] NoiseRateColor { get; set; } = new SKColor[] { SkDefault4BitColors.Grey };
}