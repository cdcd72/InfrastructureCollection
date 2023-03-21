using Infra.Core.BarCode.Models;

namespace Infra.BarCode.QRCode.Models;

public class QRCodeParam : BarCodeParam
{
    public string Text { get; set; }

    public byte[] binaryData { get; set; }

    public int Pixels { get; set; }

    public byte[] Icon { get; set; }
}
