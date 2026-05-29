using FiscalCore.Application.Abstractions;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Users;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Application.Interfaces.Users;
using FiscalCore.Domain.Interfaces.Users;
using FluentValidation;

namespace FiscalCore.Application.Services.User;

public sealed class UserService : IUserService
{
    private readonly ILogService _logService;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMessagesProvider _messagesProvider;
    private readonly IValidator<CreateUserRequest> _validatorCreateUser;
    private readonly IValidator<UpdateUserRequest> _validatorUpdateUser;



    public UserService(
        ILogService logService,
        IEncryptionService encryptionService,
        IUserRepository userRepository,
        IUnitOfWork uow,
        IMessagesProvider messagesProvider,
        IValidator<CreateUserRequest> validatorCreateUser,
        IValidator<UpdateUserRequest> validatorUpdateUser)
    {
        _logService = logService;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
        _uow = uow;
        _messagesProvider = messagesProvider;
        _validatorCreateUser = validatorCreateUser;
        _validatorUpdateUser = validatorUpdateUser;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : Map(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user is null ? null : Map(user);
    }

    public async Task<ResponseDto> CreateAsync(CreateUserRequest userRequest)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // 1 Validar request
            var validationResult = _validatorCreateUser.Validate(userRequest);

            if (!validationResult.IsValid)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validationResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                });
                return ResponseFactory.Error(_messagesProvider.GetError("UserCreateFailed"), errors);
            }

            // 2 verifica si el usuario existente
            var userExists = await _userRepository.GetByEmailAsync(userRequest.Email);
            if (userExists is not null)
            {
                IEnumerable<ResponseErrorDetailDto> errors = new[]
                {
                    new ResponseErrorDetailDto
                    {
                        Field = "Email",
                        Message = _messagesProvider.GetError("UserEmailAlreadyExists")
                    }
                };

                return ResponseFactory.Error(_messagesProvider.GetError("UserCreateFailed"), errors);
            }


            // 3 crea el usuario
            var entity = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = userRequest.Email,
                Username = userRequest.Username,
                PasswordHash = _encryptionService.Encrypt(userRequest.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(entity);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success<Guid>(entity.Id, _messagesProvider.GetMessage("UserCreatedSuccessfully"));
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(UserService)}.{nameof(CreateAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("UserCreateFailed"));
        }
    }

    public async Task<ResponseDto> UpdateAsync(UpdateUserRequest userRequest)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // 1 Validar request
            var validationResult = _validatorUpdateUser.Validate(userRequest);

            if (!validationResult.IsValid)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validationResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                });
                return ResponseFactory.Error(_messagesProvider.GetError("UserUpdateFailed"), errors);
            }

            // 2 Obtener usuario existente
            var user = await _userRepository.GetByIdAsync(userRequest.Id);

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

                return ResponseFactory.Error(_messagesProvider.GetError("UserUpdateFailed"), errors);
            }


            // 3 Validar email duplicado
            var emailExists = await _userRepository.EmailExistsAsync(userRequest.Email, user.Id);

            if (emailExists)
            {
                IEnumerable<ResponseErrorDetailDto> errors = new[]
                {
                    new ResponseErrorDetailDto
                    {
                        Field = "Email",
                        Message = _messagesProvider.GetError("UserEmailAlreadyExists")
                    }
                };

                return ResponseFactory.Error(_messagesProvider.GetError("UserUpdateFailed"), errors);
            }

            // 4 Actualizar propiedades
            user.Username = userRequest.Username;
            user.Email = userRequest.Email;
            user.IsActive = userRequest.IsActive;

            _userRepository.Update(user);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success(_messagesProvider.GetMessage("UserUpdatedSuccessfully"));

        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(UserService)}.{nameof(CreateAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("UserUpdateFailed"));
        }
    }

    public async Task<ResponseDto> DeleteAsync(Guid id)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // 1️ Obtener usuario existente
            var user = await _userRepository.GetByIdAsync(id);

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

                return ResponseFactory.Error(_messagesProvider.GetError("UserDeleteFailed"), errors);
            }                        

           
            _userRepository.Remove(user);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success(_messagesProvider.GetMessage("UserDeleteSuccessfully"));

        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(UserService)}.{nameof(CreateAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("UserDeleteFailed"));
        }
    }

    private static UserResponse Map(Domain.Entities.User u)
        => new UserResponse(u.Id, u.Email, u.Username, u.IsActive, u.CreatedAt);
}
