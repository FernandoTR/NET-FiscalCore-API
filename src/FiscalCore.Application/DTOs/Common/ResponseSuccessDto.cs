namespace FiscalCore.Application.DTOs.Common;

public class ResponseSuccessDto<T> : ResponseDto
{
    public T Data { get; init; }
}
