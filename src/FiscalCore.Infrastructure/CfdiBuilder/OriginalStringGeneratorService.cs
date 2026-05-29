using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public class OriginalStringGeneratorService : IOriginalStringGeneratorService
{
    private readonly ILogService _logService;
    private readonly IMessagesProvider _messagesProvider;

    public OriginalStringGeneratorService(ILogService logService, IMessagesProvider messagesProvider)
    {
        _logService = logService;
        _messagesProvider = messagesProvider;
    }

    public ResponseDto Generate(XDocument cfdiXml)
    {
        try
        {
            var xslt = LoadXslt();

            using var xmlReader = cfdiXml.CreateReader();
            using var stringWriter = new StringWriterWithEncoding(Encoding.UTF8);
            using var xmlWriter = XmlWriter.Create(stringWriter, xslt.OutputSettings);

            xslt.Transform(xmlReader, xmlWriter);
            string cadenaOriginal = Normalize(stringWriter.ToString());

            return ResponseFactory.Success<string>(cadenaOriginal, _messagesProvider.GetMessage("OriginalStringCreatedSuccessfully"));
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(OriginalStringGeneratorService)}.{nameof(Generate)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("ErrorGeneratingCadenaOriginal"));
        }
    }

    private XslCompiledTransform LoadXslt()
    {
        var assembly = typeof(OriginalStringGeneratorService).Assembly;

        const string baseNamespace = "FiscalCore.Infrastructure.CfdiBuilder.Schemas.Xslt";

        using var stream = assembly.GetManifestResourceStream($"{baseNamespace}.cadenaoriginal_4_0.xslt");

        if (stream is null)
        {
            _logService.ErrorLog("FiscalCore.Infrastructure.CadenaOriginalGenerator", "LoadXslt", $"No se pudo cargar el XSD embebido: cadenaoriginal_4_0.xslt");
            throw new InvalidOperationException("No se pudo cargar el XSLT oficial del SAT.");
        }

        var settings = new XsltSettings(enableDocumentFunction: false, enableScript: false);
        var resolver = new EmbeddedXsltResolver(assembly, baseNamespace);

        using var reader = XmlReader.Create(stream);

        var names = assembly.GetManifestResourceNames();

        var xslt = new XslCompiledTransform();
        xslt.Load(reader, settings, resolver);

        return xslt;
    }

    private static string Normalize(string cadena)
    {
        // El SAT exige:
        // - Sin saltos de línea
        // - Sin espacios extra
        // - Texto plano
        return cadena
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();
    }

    internal sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding => _encoding;
    }

}
