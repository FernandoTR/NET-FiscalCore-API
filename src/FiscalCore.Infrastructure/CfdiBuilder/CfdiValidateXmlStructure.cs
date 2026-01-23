using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Message;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public class CfdiValidateXmlStructure : ICfdiValidateXmlStructure
{
    private readonly IMessagesProvider _messagesProvider;
    public CfdiValidateXmlStructure(IMessagesProvider messagesProvider)
    {
        _messagesProvider = messagesProvider;
    }

    public ResponseDto Execute(XDocument xml)
    {
        var errors = new List<ResponseErrorDetailDto>();

        if (xml.Root is null)
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "cfdi:Comprobante",
                Message = "No se pudo generar el XML CFDI: documento sin nodo raíz."
            });

            return ResponseFactory.Error(_messagesProvider.GetError("InvalidCfdiStructure"), errors);
        }

        if (xml.Root.Name != CfdiNamespaces.Cfdi + "Comprobante")
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "cfdi:Comprobante",
                Message = "El XML generado no contiene el nodo cfdi:Comprobante."
            });

            return ResponseFactory.Error(_messagesProvider.GetError("InvalidCfdiStructure"), errors);
        }

        ValidateRequiredNode(xml.Root, "Emisor", errors);
        ValidateRequiredNode(xml.Root, "Receptor", errors);
        ValidateRequiredNode(xml.Root, "Conceptos", errors);

        if (errors.Any())
        {
            return ResponseFactory.Error(_messagesProvider.GetError("InvalidCfdiStructure"), errors);
        }

        return ResponseFactory.Success("La estructura del comprobante es correcta.");
    }

    private static void ValidateRequiredNode(XElement comprobante, string nodeName, ICollection<ResponseErrorDetailDto> errors)
    {
        if (!comprobante.Elements(CfdiNamespaces.Cfdi + nodeName).Any())
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = nodeName,
                Message = $"CFDI sin nodo {nodeName}."
            });
        }
    }

}
