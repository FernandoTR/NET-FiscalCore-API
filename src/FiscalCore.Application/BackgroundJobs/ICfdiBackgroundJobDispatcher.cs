
namespace FiscalCore.Application.BackgroundJobs;

public interface ICfdiBackgroundJobDispatcher
{
    void EnqueueGeneratePdfAndSendEmail(Guid cfdiId, string xmlContent, string recipientEmail);
}
