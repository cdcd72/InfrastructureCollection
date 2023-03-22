using Infra.Core.BarCode.Models;

namespace Infra.BarCode.QRCode.Models;

public class QrCodeEncodeParam : BarCodeParam
{
    public string Text { get; set; }

    public int Pixels { get; set; }
}
