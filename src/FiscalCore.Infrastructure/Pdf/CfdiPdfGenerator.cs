
using FiscalCore.Application.Interfaces.Cfdis;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.Pdf;

public sealed class CfdiPdfGenerator : ICfdiPdfGenerator
{
    private readonly IOriginalStringGeneratorService _originalStringGeneratorService;
    private readonly IQrSatService _qrSatService;

    public CfdiPdfGenerator(IOriginalStringGeneratorService originalStringGeneratorService, IQrSatService qrSatService)
    {
        _originalStringGeneratorService = originalStringGeneratorService;
        _qrSatService = qrSatService;
    }
    public Task<byte[]> GenerateAsync(string xmlContent)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        if (string.IsNullOrWhiteSpace(xmlContent))
            throw new ArgumentException("El XML CFDI es requerido", nameof(xmlContent));

        var xDocument = XDocument.Parse(xmlContent);      

        var document = new CfdiPdfDocument(xDocument, _originalStringGeneratorService, _qrSatService);

        // QuestPDF genera byte[]
        byte[] pdfBytes = document.GeneratePdf();

        // use the following invocation
        //document.ShowInCompanion();

        // optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
        //document.ShowInCompanion(12500);

        return Task.FromResult(pdfBytes);
    }
    
}
