using Infra.Core.BarCode.Models;

namespace Infra.BarCode.QRCode.Models;

public class QrCodeDecodeParam : BarCodeParam
{
    public byte[] BinaryData { get; set; }
}
