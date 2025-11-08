using System.Text;
using Newtonsoft.Json;
using SistemaVentas.Web.Services.Interfaces;

namespace SistemaVentas.Web.Services.Implementation
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _baseUrl = _configuration["ApiSettings:BaseUrl"];
            if (!_baseUrl.EndsWith("/"))
            {
                _baseUrl += "/";
            }

            _httpClient.BaseAddress = new Uri(_baseUrl);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                endpoint = endpoint.TrimStart('/');

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error en la API: {content}");
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con la API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consumir la API: {ex.Message}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                endpoint = endpoint.TrimStart('/');

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error en la API: {responseContent}");
                }

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con la API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consumir la API: {ex.Message}", ex);
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                endpoint = endpoint.TrimStart('/');

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error en la API: {responseContent}");
                }

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con la API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consumir la API: {ex.Message}", ex);
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                endpoint = endpoint.TrimStart('/');

                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error en la API: {content}");
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con la API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consumir la API: {ex.Message}", ex);
            }
        }
    }
}