using FiscalCore.Application.Interfaces.Cfdis;
using QRCoder;
using System.Globalization;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public sealed class QrSatService : IQrSatService
{
    private static readonly XNamespace CfdiNs = "http://www.sat.gob.mx/cfd/4";
    private static readonly XNamespace TfdNs = "http://www.sat.gob.mx/TimbreFiscalDigital";

    public string BuildQrUrl(XDocument cfdi)
    {
        var comprobante = cfdi.Root
            ?? throw new InvalidOperationException("XML CFDI inválido");

        var emisor = comprobante.Element(CfdiNs + "Emisor")
            ?? throw new InvalidOperationException("CFDI sin Emisor");

        var receptor = comprobante.Element(CfdiNs + "Receptor")
            ?? throw new InvalidOperationException("CFDI sin Receptor");

        var timbre = comprobante
            .Element(CfdiNs + "Complemento")?
            .Element(TfdNs + "TimbreFiscalDigital")
            ?? throw new InvalidOperationException("CFDI sin TimbreFiscalDigital");

        var uuid = timbre.Attribute("UUID")?.Value;
        var sello = timbre.Attribute("SelloCFD")?.Value;

        if (string.IsNullOrWhiteSpace(uuid) || string.IsNullOrWhiteSpace(sello))
            throw new InvalidOperationException("Timbre incompleto");

        var total = comprobante.Attribute("Total")?.Value
            ?? throw new InvalidOperationException("CFDI sin Total");

        var fe = sello.Substring(sello.Length - 8, 8);

        return
            "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx" +
            $"?id={uuid}" +
            $"&re={emisor.Attribute("Rfc")?.Value}" +
            $"&rr={receptor.Attribute("Rfc")?.Value}" +
            $"&tt={decimal.Parse(total, CultureInfo.InvariantCulture).ToString("F6", CultureInfo.InvariantCulture)}" +
            $"&fe={fe}";
    }

    public byte[] GenerateQrImage(string qrUrl)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);

        return qr.GetGraphic(20);
    }
}
