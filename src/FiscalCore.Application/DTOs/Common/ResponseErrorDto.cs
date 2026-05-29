namespace FiscalCore.Application.DTOs.Common;

public class ResponseErrorDto : ResponseDto
{
    public IEnumerable<ResponseErrorDetailDto> Errors { get; init; }
}
