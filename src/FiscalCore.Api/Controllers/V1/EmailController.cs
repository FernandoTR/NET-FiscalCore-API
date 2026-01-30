using Asp.Versioning;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiscalCore.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/email")]
[Authorize]
public class EmailController : Controller
{
    private readonly ICfdiEmailResendService _cfdiEmailResendService;
    public EmailController(ICfdiEmailResendService cfdiEmailResendService)
    {
        _cfdiEmailResendService = cfdiEmailResendService;
    }


    [HttpPost("resend")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResendEmail([FromBody] CfdiEmailResendRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("El cuerpo de la solicitud es obligatorio.");

        var result = await _cfdiEmailResendService.ResendAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

}
