using System.Reflection;
using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using NUnit.Framework;
using SixLabors.ImageSharp;

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

    [Test]
    public async Task DecodeTextWithIconSuccess()
    {
        const string text = "https://www.thinkinmd.com";

        var imageBytes = await encoder.EncodeAsync(new QrCodeEncodeParam
        {
            Text = text,
            Pixels = 400,
            Icon = new Icon
            {
                BinaryData = await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "icon.png"))
            }
        });

        var result = await decoder.DecodeAsync(new QrCodeDecodeParam
        {
            BinaryData = imageBytes
        });

        Assert.That(result, Is.EqualTo(text));
    }

    [Test]
    public async Task DecodeTextWithCustomIconSuccess()
    {
        const string text = "https://www.thinkinmd.com";

        var imageBytes = await encoder.EncodeAsync(new QrCodeEncodeParam
        {
            Text = text,
            Pixels = 400,
            Icon = new Icon
            {
                BinaryData = await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "icon.png")),
                Width = 80,
                Height = 80,
                Location = new Point(160, 160),
                Opacity = 0.8f
            }
        });

        var result = await decoder.DecodeAsync(new QrCodeDecodeParam
        {
            BinaryData = imageBytes
        });

        Assert.That(result, Is.EqualTo(text));
    }
}
