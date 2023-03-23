using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.QrCode;

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
                ErrorCorrection = param.ErrorCorrectionLevel,
                Width = param.Pixels,
                Height = param.Pixels,
                Margin = param.Margin
            }
        };

        var image = writer.WriteAsImageSharp<Rgba32>(param.Text);

        if (param.Icon is not null)
        {
            await using var iconStream = new MemoryStream(param.Icon.BinaryData);

            var icon = await Image.LoadAsync(iconStream);

            var iconWidth = param.Icon.Width ?? param.Pixels / 5;
            var iconHeight = param.Icon.Height ?? param.Pixels / 5;

            icon.Mutate(o => o.Resize(iconWidth, iconHeight));

            var iconLocation = param.Icon.Location ?? new Point((param.Pixels / 2) - (iconWidth / 2), (param.Pixels / 2) - (iconHeight / 2));

            image.Mutate(o => o.DrawImage(icon, iconLocation, param.Icon.Opacity));
        }

        await using var imageStream = new MemoryStream();

        await image.SaveAsPngAsync(imageStream);

        return imageStream.ToArray();
    }
}
