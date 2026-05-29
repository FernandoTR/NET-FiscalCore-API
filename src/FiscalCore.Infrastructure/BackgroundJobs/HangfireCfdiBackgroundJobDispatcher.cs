using FiscalCore.Application.BackgroundJobs;
using Hangfire;

namespace FiscalCore.Infrastructure.BackgroundJobs;

public sealed class HangfireCfdiBackgroundJobDispatcher : ICfdiBackgroundJobDispatcher
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireCfdiBackgroundJobDispatcher(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueGeneratePdfAndSendEmail(Guid cfdiId, string xmlContent, string recipientEmail)
    {
        var pdfJobId = _backgroundJobClient.Enqueue<GenerateAndPersistCfdiPdfJob>(
            job => job.ExecuteAsync(
                cfdiId,
                xmlContent,
                CancellationToken.None));

        _backgroundJobClient.ContinueJobWith<SendCfdiEmailJob>(
            pdfJobId,
            job => job.ExecuteAsync(
                cfdiId,
                xmlContent,
                recipientEmail,
                CancellationToken.None));
    }
}
