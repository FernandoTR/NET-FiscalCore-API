using System.Xml.Linq;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public static class CfdiNamespaces
{
    public static readonly XNamespace Cfdi = "http://www.sat.gob.mx/cfd/4";
    public static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
    public static readonly XNamespace SchemaLocation = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd";

}
