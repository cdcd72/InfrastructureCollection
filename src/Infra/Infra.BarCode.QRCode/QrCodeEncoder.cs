using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Infra.BarCode.QRCode;

public class QrCodeEncoder : IBarCodeEncoder<QrCodeEncodeParam>
{
    public async Task<byte[]> EncodeAsync(QrCodeEncodeParam param)
    {
        var writer = new BarcodeWriter<Rgba32>
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                ErrorCorrection = ErrorCorrectionLevel.Q,
                Width = param.Pixels,
                Height = param.Pixels
            }
        };

        await using var imageStream = new MemoryStream();

        await writer
            .WriteAsImageSharp<Rgba32>(param.Text)
            .SaveAsPngAsync(imageStream);

        return imageStream.ToArray();
    }
}
