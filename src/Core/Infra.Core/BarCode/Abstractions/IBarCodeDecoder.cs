using Infra.Core.BarCode.Models;

namespace Infra.Core.BarCode.Abstractions;

public interface IBarCodeDecoder<in TBarCodeDecodeParam> where TBarCodeDecodeParam : BarCodeParam
{
    Task<string> DecodeAsync(TBarCodeDecodeParam param);
}
