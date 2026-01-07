using Fundo.Services.Enums;

namespace Fundo.Services.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public ServiceErrorType ErrorType { get; set; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> NotFound(string message) => new() { Success = false, ErrorMessage = message, ErrorType = ServiceErrorType.NotFound };
    public static ServiceResult<T> BadRequest(string message) => new() { Success = false, ErrorMessage = message, ErrorType = ServiceErrorType.BadRequest };
}
