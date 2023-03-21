using Infra.Core.BarCode.Models;

namespace Infra.Core.BarCode.Abstractions;

public interface IBarCodeGenerator<in TBarCodeParam> where TBarCodeParam : BarCodeParam
{
    Task<byte[]> CreateAsync(TBarCodeParam barCodeParam);
}
