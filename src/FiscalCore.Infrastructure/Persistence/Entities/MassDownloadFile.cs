namespace FiscalCore.Infrastructure.Persistence.Entities;

public partial class MassDownloadFile
{
    public Guid Id { get; set; }

    public Guid RequestId { get; set; }

    public Guid Uuid { get; set; }

    public string FilePath { get; set; } = null!;

}
