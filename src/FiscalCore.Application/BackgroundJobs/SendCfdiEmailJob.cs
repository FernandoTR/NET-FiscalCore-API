using FiscalCore.Application.DTOs.Email;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Email;
using FiscalCore.Application.Interfaces.Logging;

namespace FiscalCore.Application.BackgroundJobs;

public sealed class SendCfdiEmailJob
{
    private readonly IEmailService _emailService;
    private readonly ICfdiPdfStore _cfdiPdfStore;
    private readonly ILogService _logService;
    public SendCfdiEmailJob(
        IEmailService emailService,
        ICfdiPdfStore cfdiPdfStore,
        ILogService logService)
    {
        _emailService = emailService;
        _cfdiPdfStore = cfdiPdfStore;
        _logService = logService;
    }

    public async Task ExecuteAsync(Guid cfdiId, string xmlContent, string recipientEmail, CancellationToken ct)
    {
        var pdf = await _cfdiPdfStore.GetByCfdiIdAsync(cfdiId);

        if (pdf is null)
            throw new InvalidOperationException(
                $"PDF not found for CFDI {cfdiId}");


        try
        {
            await _emailService.SendAsync(
               to: recipientEmail,
               subject: "CFDI generado",
               body: "Se adjuntan los archivos XML y PDF de su CFDI.",
               attachments: new[]
               {
                    EmailAttachment.FromBytes(
                        $"{cfdiId}.xml",
                        System.Text.Encoding.UTF8.GetBytes(xmlContent),
                        "application/xml"
                    ),
                    EmailAttachment.FromFile(
                        pdf.FilePath,
                        contentType: "application/pdf"
                    )
               },
               ct);
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(SendCfdiEmailJob)}.{nameof(ExecuteAsync)}", ex);
            throw;
        }
       

        
    }
}
