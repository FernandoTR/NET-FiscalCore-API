namespace FiscalCore.Application.DTOs.Common;

public static class ResponseFactory
{
    /* ===================== */
    /* SUCCESS RESPONSES     */
    /* ===================== */


    public static ResponseSuccessDto<T> Success<T>(T data, string message = null)
    {
        return new ResponseSuccessDto<T>
        {
            IsSuccess = true,
            Message = message ?? "Operación exitosa.",
            Data = data
        };
    }

    public static ResponseDto Success(string message)
    {
        return new ResponseDto
        {
            IsSuccess = true,
            Message = message
        };
    }

    /* ===================== */
    /* ERROR RESPONSES       */
    /* ===================== */

    public static ResponseErrorDto Error(string message,  IEnumerable<ResponseErrorDetailDto> errors = null)
    {
        return new ResponseErrorDto
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? Enumerable.Empty<ResponseErrorDetailDto>()
        };
    }

    public static ResponseErrorDto Exception(Exception exception, string message)
    {
        return new ResponseErrorDto
        {
            IsSuccess = false,
            Message = message,
            Errors = new[]
            {
                new ResponseErrorDetailDto
                {
                    Field = "Exception",
                    Message = exception.Message
                }
            }
        };
    }
}