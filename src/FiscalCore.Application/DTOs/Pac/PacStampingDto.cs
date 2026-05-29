namespace FiscalCore.Application.DTOs.Pac;

public sealed class PacStampingDto
{
    public bool IsSuccess { get; init; }
    public string Uuid { get; init; }
    public string Xml { get; init; }
    public string Menssage { get; init; }
}
