namespace Talabat.API.Errors
{
    public class ApiExceptionError: ApiResponse
    {
        public string? Details { get; set; }
        public ApiExceptionError(int StatusCode,string? Message = null, string? details = null):base(StatusCode,Message)
        {
            Details = details;
        }
    }
}
