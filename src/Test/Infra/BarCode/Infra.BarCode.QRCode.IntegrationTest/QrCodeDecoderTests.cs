using System.Reflection;
using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using NUnit.Framework;

namespace Infra.BarCode.QRCode.IntegrationTest;

public class QrCodeDecoderTests
{
    private readonly IBarCodeEncoder<QrCodeEncodeParam> encoder;
    private readonly IBarCodeDecoder<QrCodeDecodeParam> decoder;

    #region Properties

    private static string CurrentDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    #endregion

    public QrCodeDecoderTests()
    {
        var startup = new Startup();

        encoder = startup.GetService<IBarCodeEncoder<QrCodeEncodeParam>>();
        decoder = startup.GetService<IBarCodeDecoder<QrCodeDecodeParam>>();
    }

    [Test]
    public async Task DecodeTextSuccess()
    {
        const string text = "https://www.thinkinmd.com";

        var imageBytes = await encoder.EncodeAsync(new QrCodeEncodeParam
        {
            Text = text,
            Pixels = 400
        });

        var result = await decoder.DecodeAsync(new QrCodeDecodeParam
        {
            BinaryData = imageBytes
        });

        Assert.That(result, Is.EqualTo(text));
    }
}
