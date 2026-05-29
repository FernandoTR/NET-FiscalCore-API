namespace FiscalCore.Application.Interfaces.Pac;

public interface IPacAuthService
{
    Task<string> GetTokenAsync(CancellationToken ct);
}
