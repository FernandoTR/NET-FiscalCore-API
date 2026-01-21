
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Users;

namespace FiscalCore.Application.Interfaces.Users;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<UserResponse> GetByEmailAsync(string email);
    Task<ResponseDto> CreateAsync(CreateUserRequest userRequest);
    Task<ResponseDto> UpdateAsync(UpdateUserRequest userRequest);
    Task<ResponseDto> DeleteAsync(Guid id);
}
