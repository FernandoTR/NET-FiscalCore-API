using FiscalCore.Application.DTOs.Pac;
using FiscalCore.Application.Interfaces.Pac;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.Pac;

/// <summary>
/// Funciona como un servicio de timbrado falso para propósitos de prueba.
/// </summary>
public sealed class FakeStampingService : IPacStampingService
{
    public Task<PacStampingDto> StampingXmlAsync(string cfdiXml, CancellationToken ct)
    {
        var doc = XDocument.Parse(cfdiXml);

        XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
        XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

        var complemento = doc
            .Root!
            .Elements(cfdi + "Complemento")
            .FirstOrDefault();

        if (complemento == null)
        {
            complemento = new XElement(cfdi + "Complemento");
            doc.Root!.Add(complemento);
        }

        var uuid = Guid.NewGuid().ToString().ToUpper();

        var timbre = new XElement(tfd + "TimbreFiscalDigital",
            new XAttribute("Version", "1.1"),
            new XAttribute("UUID", uuid),
            new XAttribute("FechaTimbrado", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XAttribute("RfcProvCertif", "AAA010101AAA"),
            new XAttribute("SelloCFD", Convert.ToBase64String(RandomBytes(128))),
            new XAttribute("NoCertificadoSAT", "00000000000000000000"),
            new XAttribute("SelloSAT", Convert.ToBase64String(RandomBytes(256)))
        );

        complemento.Add(timbre);

        // Esta estructura es exactamente la definida por el SAT, solo que con datos ficticios
        var stampedXml = doc.Declaration + doc.ToString(SaveOptions.DisableFormatting);

        return Task.FromResult(new PacStampingDto
        {
            IsSuccess = true,
            Uuid = uuid,
            Xml = stampedXml,
            Menssage = "Timbrado exitoso"
        });
    }

    private static byte[] RandomBytes(int length)
    {
        var buffer = new byte[length];
        RandomNumberGenerator.Fill(buffer);
        return buffer;
    }

}
