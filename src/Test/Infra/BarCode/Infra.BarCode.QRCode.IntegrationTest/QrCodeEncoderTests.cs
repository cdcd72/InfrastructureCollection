using System.Reflection;
using Infra.BarCode.QRCode.Models;
using Infra.Core.BarCode.Abstractions;
using NUnit.Framework;

namespace Infra.BarCode.QRCode.IntegrationTest;

public class QrCodeEncoderTests
{
    private readonly IBarCodeEncoder<QrCodeEncodeParam> encoder;

    #region Properties

    private static string CurrentDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    #endregion

    public QrCodeEncoderTests()
    {
        var startup = new Startup();

        encoder = startup.GetService<IBarCodeEncoder<QrCodeEncodeParam>>();
    }

    [Test]
    public async Task EncodeTextSuccess()
    {
        const string text = "https://www.thinkinmd.com";

        var imageBytes = await encoder.EncodeAsync(new QrCodeEncodeParam
        {
            Text = text,
            Pixels = 400
        });

        Assert.That(imageBytes, Is.Not.Empty);
    }
}
