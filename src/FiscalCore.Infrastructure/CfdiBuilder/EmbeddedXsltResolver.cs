using System.Net;
using System.Reflection;
using System.Xml;

namespace FiscalCore.Infrastructure.CfdiBuilder;
public sealed class EmbeddedXsltResolver : XmlResolver
{
    private readonly Assembly _assembly;
    private readonly string _baseNamespace;

    public EmbeddedXsltResolver(Assembly assembly, string baseNamespace)
    {
        _assembly = assembly;
        _baseNamespace = baseNamespace;
    }

    public override object GetEntity(
        Uri absoluteUri,
        string role,
        Type ofObjectToReturn)
    {
        // xsl:include href="utilerias.xslt"
        var fileName = Path.GetFileName(absoluteUri.OriginalString);

        var resourceName = $"{_baseNamespace}.{fileName}";

        var stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException(
                $"XSLT embebido no encontrado: {resourceName}"
            );

        return stream;
    }

    public override ICredentials Credentials
    {
        set { }
    }
}