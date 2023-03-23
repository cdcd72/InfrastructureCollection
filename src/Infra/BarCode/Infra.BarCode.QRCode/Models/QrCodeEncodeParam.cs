using Infra.Core.BarCode.Models;
using SixLabors.ImageSharp;
using ZXing.QrCode.Internal;

namespace Infra.BarCode.QRCode.Models;

public class QrCodeEncodeParam : BarCodeParam
{
    public string Text { get; set; }

    public ErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = ErrorCorrectionLevel.Q;

    public int Pixels { get; set; }

    public int Margin { get; set; } = 1;

    public Icon Icon { get; set; }
}

public class Icon
{
    public byte[] BinaryData { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public Point? Location { get; set; }

    public float Opacity { get; set; } = 1f;
}
