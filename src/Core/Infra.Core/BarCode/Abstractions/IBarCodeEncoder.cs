using Infra.Core.BarCode.Models;

namespace Infra.Core.BarCode.Abstractions;

public interface IBarCodeEncoder<in TBarCodeEncodeParam> where TBarCodeEncodeParam : BarCodeParam
{
    Task<byte[]> EncodeAsync(TBarCodeEncodeParam param);
}
