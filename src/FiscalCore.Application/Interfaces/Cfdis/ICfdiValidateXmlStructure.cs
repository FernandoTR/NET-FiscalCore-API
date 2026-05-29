using FiscalCore.Application.DTOs.Common;
using System.Xml.Linq;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiValidateXmlStructure
{
    ResponseDto Execute(XDocument xml);
}
