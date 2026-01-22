using FiscalCore.Application.DTOs.Pac;
using FiscalCore.Application.Interfaces.Pac;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace FiscalCore.Infrastructure.Pac;

public sealed class PaxAuthService : IPacAuthService
{
    private readonly HttpClient _http;
    private readonly string _username;
    private readonly string _password;

    public PaxAuthService(HttpClient http, IOptions<PacOptions> options)
    {
        _http = http;
        _username = options.Value.Username;
        _password = options.Value.Password;
    }

    public async Task<string> GetTokenAsync(CancellationToken ct)
    {
        var request = new PacAuthRequest
        {
            Username = _username,
            Password = _password
        };

        var response = await _http.PostAsJsonAsync("/api/login/autenticacion", request, ct);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PacAuthResponse>(ct);

        return payload.Resultado.Token.Datos;
    }
}