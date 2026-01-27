using System.Reflection;


namespace FiscalCore.Infrastructure.Pdf;

public static class ResourceImageLoader
{
    public static byte[] Load(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var fullName = $"FiscalCore.Infrastructure.Resources.{resourceName}";

        using var stream = assembly.GetManifestResourceStream(fullName);

        if (stream == null)
            throw new FileNotFoundException($"Recurso no encontrado: {fullName}");

        using var ms = new MemoryStream();
        stream.CopyTo(ms);

        return ms.ToArray();
    }
}
