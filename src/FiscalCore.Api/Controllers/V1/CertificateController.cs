using Asp.Versioning;
using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Certificate;
using FiscalCore.Application.Interfaces.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Api.Controllers.V1;

/// <summary>
/// Administración de Certificados (CSD / FIEL)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/certificate")]
[Authorize]
public class CertificateController : Controller
{
    private readonly ICertificateService _certificateService;
    private readonly IMessagesProvider _messagesProvider;

    public CertificateController(ICertificateService certificateService, IMessagesProvider messagesProvider)
    {
        _certificateService = certificateService;
        _messagesProvider = messagesProvider;
    }

    /// <summary>
    /// Obtiene un certificado por su identificador
    /// </summary>
    [HttpGet("{certificateId:guid}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<CertificateRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid certificateId, CancellationToken cancellationToken)
    {
        var certificate = await _certificateService.GetByIdAsync(certificateId);

        if (certificate is null)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = _messagesProvider.GetError("CertificateNotFound2")
                }
            };

            return NotFound(ResponseFactory.Error(_messagesProvider.GetError("CertificateNotFound2"), errors));
        }

        return Ok(ResponseFactory.Success(certificate, _messagesProvider.GetMessage("CertificateRetrievedSuccessfully")));        
    }

    /// <summary>
    /// Obtiene los certificados asociados a un usuario
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<IReadOnlyList<CertificateRequest>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var certificates = await _certificateService.GetByUserAsync(userId);

        if (certificates is null)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
             {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = _messagesProvider.GetError("CertificateNotFound2")
                }
            };

            return NotFound(ResponseFactory.Error(_messagesProvider.GetError("CertificateNotFound2"), errors));
        }

        return Ok(ResponseFactory.Success(certificates, _messagesProvider.GetMessage("CertificateRetrievedSuccessfully")));
    }

    /// <summary>
    /// Obtiene el certificado activo por RFC
    /// </summary>
    [HttpGet("active/{rfc}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<CertificateRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveByRfc(string rfc, CancellationToken cancellationToken)
    {
        var certificate = await _certificateService.GetActiveByRfcAsync(rfc);

        if (certificate is null)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = _messagesProvider.GetError("CertificateNotFound3")
                }
            };

            return NotFound(ResponseFactory.Error(_messagesProvider.GetError("CertificateNotFound3"), errors));
        }

        return Ok(ResponseFactory.Success(certificate, _messagesProvider.GetMessage("CertificateRetrievedSuccessfully")));        
    }

    /// <summary>
    /// Registro de un nuevo certificado
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCertificateRequest certificate, CancellationToken cancellationToken)
    {
        var response = await _certificateService.CreateAsync(certificate);

        if (!response.IsSuccess)
            return BadRequest(response);


        return CreatedAtAction(
           nameof(GetById),
           new { certificateId = ((ResponseSuccessDto<Guid>)response).Data },
           response);
    }

    /// <summary>
    /// Actualización de un certificado existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCertificateRequest certificate, CancellationToken cancellationToken)
    {
        if (id != certificate.CertificateId)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = "El ID de ruta no coincide con el ID del cuerpo."
                }
            };

            return BadRequest(ResponseFactory.Error(_messagesProvider.GetError("CertificateUpdateFailed"), errors));
        }

        var response = await _certificateService.UpdateAsync(certificate);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>
    /// Eliminación de un certificado
    /// </summary>
    [HttpDelete("{certificateId:guid}")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid certificateId, CancellationToken cancellationToken)
    {
        var response = await _certificateService.DeleteAsync(certificateId);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


}
