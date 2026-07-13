namespace ApiClientLib
{
    public abstract class ApiResponseBase
    {
        protected ApiResponseBase(int statusCode)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}