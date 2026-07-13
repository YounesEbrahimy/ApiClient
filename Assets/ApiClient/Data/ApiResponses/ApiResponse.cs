namespace ApiClientLib
{
    public sealed class ApiResponse<T> : ApiResponseBase
    {
        public ApiResponse(int statusCode, T data) : base(statusCode)
        {
            Data = data;
        }

        public T Data { get; }
    }
}