namespace PAYUNiSDK.Models;

public class PAYUNiException : Exception
{
    public ErrorCodes ErrorCode { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public object ReturnValue { get; set; } = null;

    public PAYUNiException(ErrorCodes errorCode)
    {
        ErrorCode = errorCode;
    }

    public PAYUNiException(ErrorCodes errorCode, HttpStatusCode? statusCode = HttpStatusCode.OK) : base(errorCode.GetDescription())
    {
        ErrorCode = errorCode;
        StatusCode = statusCode ?? HttpStatusCode.OK;
    }

    public PAYUNiException(ErrorCodes errorCode, Exception ex) : base(ex.Message, ex.InnerException)
    {
        ErrorCode = errorCode;
    }
}
