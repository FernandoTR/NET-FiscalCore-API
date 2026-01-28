using Asp.Versioning;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Services.Cfdi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiscalCore.Api.Controllers.V1;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cfdi")]
[Authorize]
public class CfdiController : Controller
{
    private readonly ICreateAndStampCfdiService _createAndStampCfdiService;
    private readonly ICfdiQueryService _cfdiQueryService;

    public CfdiController(ICreateAndStampCfdiService createAndStampCfdiService, ICfdiQueryService cfdiQueryService)
    {
        _createAndStampCfdiService = createAndStampCfdiService;
        _cfdiQueryService = cfdiQueryService;
    }


    /// <summary>
    /// Timbrado de CFDI 4.0 a partir de JSON
    /// FiscalFlow genera XML, sello y timbre
    /// </summary>
    [HttpPost("timbrar")]
    [ProducesResponseType(typeof(ResponseSuccessDto<CreateCfdiResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Timbrar([FromBody] CreateCfdiRequest request, CancellationToken cancellationToken)
    {
        var result = await _createAndStampCfdiService.ExecuteAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


    /// <summary>
    /// Recupera un CFDI por su UUID (Folio Fiscal SAT)
    /// </summary>
    [HttpGet("{uuid:guid}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<CfdiFullResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByUuid(Guid uuid)
    {
        var result = await _cfdiQueryService.GetByUuidAsync(uuid);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


}
