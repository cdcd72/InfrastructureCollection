using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp;
using ZXing.QrCode;

namespace Infra.BarCode.QRCode;

public class QrCodeDecoder : IBarCodeDecoder<QrCodeDecodeParam>
{
    public async Task<string> DecodeAsync(QrCodeDecodeParam param)
    {
        await using var imageStream = new MemoryStream(param.BinaryData);

        var image = await Image.LoadAsync<Rgba32>(imageStream);

        var imageSharpLuminanceSource = new ImageSharpLuminanceSource<Rgba32>(image);

        var reader = new QRCodeReader();

        var result = reader.decode(new BinaryBitmap(new HybridBinarizer(imageSharpLuminanceSource)));

        return result?.Text;
    }
}
