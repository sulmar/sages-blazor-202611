using QRCoder;

namespace BlazorApp.Endpoints;

public static class QrCodeEndpoints
{
    private const int MaxDataLength = 500;

    public static IEndpointConventionBuilder MapQrCode(this IEndpointRouteBuilder app) =>
        // GET api/qrcode?data=
        app.MapGet("/api/qrcode", (string data) =>
        {
            if (string.IsNullOrWhiteSpace(data) || data.Length > MaxDataLength)
                return Results.BadRequest($"Parametr data jest wymagany i może mieć maksymalną długosc {MaxDataLength} znaków");

            using var generator = new QRCodeGenerator();
            using var qrData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);

            var png = new PngByteQRCode(qrData).GetGraphic(pixelsPerModule: 10);

            return Results.File(png, "image/png");
        });
}
