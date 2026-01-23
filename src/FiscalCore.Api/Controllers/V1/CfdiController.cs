using Asp.Versioning;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
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

    public CfdiController(ICreateAndStampCfdiService createAndStampCfdiService)
    {
        _createAndStampCfdiService = createAndStampCfdiService;
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




}
