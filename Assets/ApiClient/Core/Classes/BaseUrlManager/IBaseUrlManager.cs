namespace ApiClientLib.SubClasses
{
    internal interface IBaseUrlManager
    {
        string BaseUrl { get; }
        void SetBaseUrl(string baseUrl);
    }
}