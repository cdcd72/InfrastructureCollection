# Infra.BarCode.QRCode

透過 ZXing.Net 實現 QRCode 編/解碼機制。  
Implement qrcode encode/decode mechanism with ZXing.Net.

## How to use

> 新增 QRCode 編/解碼器實例至 DI 容器中。

1. Add QRCode encoder or decoder instance to DI container

    ```csharp
    // QRCode encoder
    builder.Services.AddSingleton<IBarCodeEncoder<QrCodeEncodeParam>, QrCodeEncoder>();
    // QRCode decoder
    builder.Services.AddSingleton<IBarCodeDecoder<QrCodeDecodeParam>, QrCodeDecoder>();
    ```

> 注入 `IBarCodeEncoder<QrCodeEncodeParam>` 或 `IBarCodeDecoder<QrCodeDecodeParam>` 來使用 QRCode 編/解碼器。

2. Inject `IBarCodeEncoder<QrCodeEncodeParam>` or `IBarCodeDecoder<QrCodeDecodeParam>` to use QRCode encoder or decoder.
