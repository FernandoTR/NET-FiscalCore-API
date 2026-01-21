using Asp.Versioning;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Users;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Users;
using FiscalCore.Domain.Interfaces.Users;
using FiscalCore.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FiscalCore.Api.Controllers.V1;

/// <summary>
/// Administración de Usuarios
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/User")]
[Authorize]
public class UserController : Controller
{  
    private readonly IMessagesProvider _messagesProvider;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IMessagesProvider messagesProvider)
    {
        _userService = userService;
        _messagesProvider = messagesProvider;
    }

    /// <summary>
    /// Registro de un nuevo usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseSuccessDto<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var response = await _userService.CreateAsync(request);

        if(!response.IsSuccess)
            return BadRequest(response);


        return CreatedAtAction(
            nameof(GetById),
            new { id = ((ResponseSuccessDto<Guid>)response).Data },
            response);
    }

    /// <summary>
    /// Actualización de un usuario
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id,[FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        if (id != request.Id)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = "El ID de ruta no coincide con el ID del cuerpo."
                }
            };

            return BadRequest(ResponseFactory.Error(_messagesProvider.GetError("UserUpdateFailed"), errors));
        }

        var response = await _userService.UpdateAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response);
       
       return Ok(response);
    }

    /// <summary>
    /// Eliminación de un usuario
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var response = await _userService.DeleteAsync(id);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    /// <summary>
    /// Obtiene un usuario por su identificador
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResponseSuccessDto<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            IEnumerable<ResponseErrorDetailDto> errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Id",
                    Message = _messagesProvider.GetError("UserNotFound")
                }
            };
            
            return NotFound(ResponseFactory.Error(_messagesProvider.GetError("UserNotFound"), errors));
        }

        
        return Ok(ResponseFactory.Success(user, _messagesProvider.GetMessage("UserFound")));
    }




}
