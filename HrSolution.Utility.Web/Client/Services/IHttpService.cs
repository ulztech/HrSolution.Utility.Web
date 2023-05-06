namespace HrSolution.Utility.Web.Client.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri);
        Task<T> Post<T>(string uri, T value);

        Task<byte[]> GetStream(string uri);
    }
}
