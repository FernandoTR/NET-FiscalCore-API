using System.Xml.Linq;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface IQrSatService
{
    string BuildQrUrl(XDocument cfdi);
    byte[] GenerateQrImage(string qrUrl);
}
