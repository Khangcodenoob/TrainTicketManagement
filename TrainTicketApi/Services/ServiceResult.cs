namespace TrainTicketApi.Services;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T? data, string message = "Success") =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, Message = message, Data = data };

    public static ServiceResult<T> Created(T? data, string message = "Created") =>
        new() { Success = true, StatusCode = StatusCodes.Status201Created, Message = message, Data = data };

    public static ServiceResult<T> Fail(int statusCode, string message) =>
        new() { Success = false, StatusCode = statusCode, Message = message };
}
