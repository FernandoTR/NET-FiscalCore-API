using FiscalCore.Application.DTOs.Cfdis;
using System.Xml.Linq;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiXmlBuilder
{
    XDocument Build(CreateCfdiRequest cfdi);
}
