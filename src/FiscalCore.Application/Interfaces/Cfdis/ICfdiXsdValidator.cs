
using FiscalCore.Application.DTOs.Common;
using System.Xml.Linq;

namespace FiscalCore.Application.Interfaces.Cfdis;

public interface ICfdiXsdValidator
{
    ResponseDto Validate(XDocument xml);
}
