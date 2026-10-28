namespace MundoRuta.Servicios.ServiciosHTTP
{
    public interface IHttpServices
    {
        Task<HttpResp<T>> GetAsync<T>(string url);

        Task<HttpResp<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request);

        Task<HttpResp<TResponse>> PostAsync<TResponse>(string url);

        Task<HttpResp<object>> PostAsync<TRequest>(string url, TRequest request);

        Task<HttpResp<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest request);

        Task<HttpResp<TResponse>> PutAsync<TResponse>(string url);

        Task<HttpResp<object>> PutAsync<TRequest>(string url, TRequest request);

        Task<HttpResp<object>> PutAsync(string url);

        Task<HttpResp<TResponse>> DeleteAsync<TResponse>(string url);

        Task<HttpResp<object>> DeleteAsync(string url);
    }
}
