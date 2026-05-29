
namespace FiscalCore.Application.DTOs.Cfdis;

public sealed record CfdiEmailResendRequest(
    Guid Uuid,
    string To
);