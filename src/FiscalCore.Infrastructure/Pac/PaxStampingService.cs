using FiscalCore.Application.DTOs.Pac;
using FiscalCore.Application.Interfaces.Pac;
using System.Net.Http.Headers;
using System.Text;

namespace FiscalCore.Infrastructure.Pac;

public sealed class PaxStampingService : IPacStampingService
{
    private readonly HttpClient _http;
    private readonly IPacAuthService _auth;

    public PaxStampingService(HttpClient http, IPacAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<PacStampingDto> StampingXmlAsync(string cfdiXml, CancellationToken ct)
    {
        var token = await _auth.GetTokenAsync(ct);

        var request = new HttpRequestMessage(HttpMethod.Post, "/CFDI/40/timbrar");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        request.Content = new StringContent(cfdiXml, Encoding.UTF8, "application/xml");

        var response = await _http.SendAsync(request, ct);
        var raw = await response.Content.ReadAsStringAsync(ct);


        if (!response.IsSuccessStatusCode)
        {
            return new PacStampingDto
            {
                IsSuccess = false,
                Menssage = raw
            };
        }

        return ParsePaxTimbradoResponse(raw);
    }

    private PacStampingDto ParsePaxTimbradoResponse(string raw)
    {
        // La guía indica que puede ser JSON o XML
        throw new NotImplementedException();
    }
}

