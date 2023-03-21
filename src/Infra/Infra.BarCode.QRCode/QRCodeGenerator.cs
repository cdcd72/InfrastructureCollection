using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Infra.BarCode.QRCode;

public class QRCodeGenerator : IBarCodeGenerator<QRCodeParam>
{
    public async Task<byte[]> CreateAsync(QRCodeParam barCodeParam)
    {
        const QRCoder.QRCodeGenerator.ECCLevel eccLevel = QRCoder.QRCodeGenerator.ECCLevel.Q;

        Image image;

        using (var qrGenerator = new QRCoder.QRCodeGenerator())
        {
            var qrCodeData = barCodeParam.binaryData is not null
                ? qrGenerator.CreateQrCode(barCodeParam.binaryData, eccLevel)
                : qrGenerator.CreateQrCode(barCodeParam.Text, eccLevel);

            using (var qrCode = new QRCoder.QRCode(qrCodeData))
            {
                if (barCodeParam.Icon is not null)
                {
                    var icon = await Image.LoadAsync(new MemoryStream(barCodeParam.Icon));

                    image = qrCode.GetGraphic(barCodeParam.Pixels, Color.Black, Color.White, icon);
                }
                else
                {
                    image = qrCode.GetGraphic(barCodeParam.Pixels);
                }
            }
        }

        using var ms = new MemoryStream();

        await image.SaveAsync(ms, new PngEncoder());

        return ms.ToArray();
    }
}
