using Asp.Versioning;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiscalCore.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cfdiPdf")]
[Authorize]

public class CfdiPdfController : Controller
{
    private readonly ICfdiPdfGenerateOrRegenerateService _cfdiPdfGenerateOrRegenerateService;

    public CfdiPdfController(ICfdiPdfGenerateOrRegenerateService cfdiPdfGenerateOrRegenerateService)
    {
        _cfdiPdfGenerateOrRegenerateService = cfdiPdfGenerateOrRegenerateService;
    }

    /// <summary>
    /// Genera/Regenera una nueva versión del PDF del CFDI.
    /// </summary>
    [HttpGet("{uuid:guid}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<CfdiPdfGenerateOrRegenerateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(Guid uuid, CancellationToken cancellationToken)
    {
        var response = await _cfdiPdfGenerateOrRegenerateService.ExecuteAsync(uuid, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }





}
