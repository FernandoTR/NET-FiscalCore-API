
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public sealed class CfdiXsdValidator : ICfdiXsdValidator
{
    private readonly ILogService _logService;
    private readonly IMessagesProvider _messagesProvider;

    public CfdiXsdValidator(ILogService logService, IMessagesProvider messagesProvider)
    {
        _logService = logService;
        _messagesProvider = messagesProvider;
    }

    public ResponseDto Validate(XDocument xml)
    {
        var errors = new List<ResponseErrorDetailDto>();
        var schemas = LoadSchemas(errors);

        xml.Validate(
            schemas,
            (sender, e) =>
            {
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = ExtractField(e),
                    Message = e.Message
                });
            },
            true
        );

        if (errors.Any())
            return ResponseFactory.Error(_messagesProvider.GetError("CfdiXsdValidationFailed"), errors);


        return ResponseFactory.Success("Los xsd de cargaron correctamente.");
    }

    private XmlSchemaSet LoadSchemas(List<ResponseErrorDetailDto> errors)
    {
        var schemaSet = new XmlSchemaSet();

        try
        {
            var assembly = typeof(CfdiXsdValidator).Assembly;

            var xsdResources = new[]
            {
                "FiscalCore.Infrastructure.CfdiBuilder.Schemas.Cfdi40.cfdv40.xsd",
                "FiscalCore.Infrastructure.CfdiBuilder.Schemas.Cfdi40.tdCFDI.xsd",
                "FiscalCore.Infrastructure.CfdiBuilder.Schemas.Cfdi40.catCFDI.xsd",
                "FiscalCore.Infrastructure.CfdiBuilder.Schemas.TimbreFiscal.TimbreFiscalDigitalv11.xsd"
            };

            foreach (var resource in xsdResources)
            {
                using var stream = assembly.GetManifestResourceStream(resource);

                if (stream is null)
                {
                    _logService.ErrorLog($"{nameof(CfdiXsdValidator)}", $"{nameof(LoadSchemas)}", $"No se pudo cargar el XSD embebido: {resource}");
                    errors.Add(new ResponseErrorDetailDto
                    {
                        Field = "XSD",
                        Message = $"No se pudo cargar el XSD embebido: {resource}"
                    });                   
                    break;
                }

                using var reader = XmlReader.Create(stream);
                schemaSet.Add(null, reader);
            }

            schemaSet.Compile();
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(CfdiXsdValidator)}.{nameof(LoadSchemas)}", ex);
        }

        return schemaSet;
    }

    private static string ExtractField(ValidationEventArgs e)
    {
        if (e.Exception is XmlSchemaValidationException ex)
        {
            return !string.IsNullOrWhiteSpace(ex.SourceObject?.ToString())
                ? ex.SourceObject.ToString()!
                : "XML";
        }

        return "XML";
    }
}
