using System.Net.Http.Json;

namespace MundoRuta.Servicios.ServiciosHTTP
{
    public class HttpServices : IHttpServices
    {
        private readonly HttpClient http;

        public HttpServices(HttpClient http)
        {
            this.http = http;
        }

        public async Task<HttpResp<T>> GetAsync<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<T>(default, true, response, null);
                }

                T? dato = await response.Content.ReadFromJsonAsync<T>();
                return new HttpResp<T>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<T>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            try
            {
                HttpResponseMessage response = await http.PostAsJsonAsync(url, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResponse>(default, true, response, null);
                }

                TResponse? dato = await response.Content.ReadFromJsonAsync<TResponse>();
                return new HttpResp<TResponse>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<TResponse>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResponse>> PostAsync<TResponse>(string url)
        {
            try
            {
                HttpResponseMessage response = await http.PostAsync(url, null);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResponse>(default, true, response, null);
                }

                TResponse? dato = await response.Content.ReadFromJsonAsync<TResponse>();
                return new HttpResp<TResponse>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<TResponse>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<object>> PostAsync<TRequest>(string url, TRequest request)
        {
            try
            {
                HttpResponseMessage response = await http.PostAsJsonAsync(url, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<object>(default, true, response, null);
                }

                return new HttpResp<object>(null, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<object>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest request)
        {
            try
            {
                HttpResponseMessage response = await http.PutAsJsonAsync(url, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResponse>(default, true, response, null);
                }

                TResponse? dato = await response.Content.ReadFromJsonAsync<TResponse>();
                return new HttpResp<TResponse>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<TResponse>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResponse>> PutAsync<TResponse>(string url)
        {
            try
            {
                HttpResponseMessage response = await http.PutAsync(url, null);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResponse>(default, true, response, null);
                }

                TResponse? dato = await response.Content.ReadFromJsonAsync<TResponse>();
                return new HttpResp<TResponse>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<TResponse>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<object>> PutAsync<TRequest>(string url, TRequest request)
        {
            try
            {
                HttpResponseMessage response = await http.PutAsJsonAsync(url, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<object>(default, true, response, null);
                }

                return new HttpResp<object>(null, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<object>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<object>> PutAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await http.PutAsync(url, null);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<object>(default, true, response, null);
                }

                return new HttpResp<object>(null, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<object>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResponse>> DeleteAsync<TResponse>(string url)
        {
            try
            {
                HttpResponseMessage response = await http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResponse>(default, true, response, null);
                }

                if (response.Content.Headers.ContentLength is null or 0)
                {
                    return new HttpResp<TResponse>(default, false, response, null);
                }

                TResponse? dato = await response.Content.ReadFromJsonAsync<TResponse>();
                return new HttpResp<TResponse>(dato, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<TResponse>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<object>> DeleteAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<object>(default, true, response, null);
                }

                return new HttpResp<object>(null, false, response, null);
            }
            catch (Exception e)
            {
                return new HttpResp<object>(default, true, null, e.Message);
            }
        }
    }
}
