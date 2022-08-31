using System.Net.Http.Json;

namespace BloodPressure.Client.Services.MeasurementService;

public class MeasurementService : IMeasurementService
{
    private readonly HttpClient _http;

    public MeasurementService(HttpClient http) => _http = http;

    public async Task<ServiceResponse<Measurement>> Get(int id)
        => await _http.GetFromJsonAsync<ServiceResponse<Measurement>>($"api/measurement/{id}");

    public async Task<ServiceResponse<IEnumerable<Measurement>>> GetAll()
        => await _http.GetFromJsonAsync<ServiceResponse<IEnumerable<Measurement>>>("api/measurement");

    public async Task<ServiceResponse<bool>> Insert(Measurement request)
    {
        using HttpResponseMessage? result = await _http.PostAsJsonAsync("api/measurement", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> Update(Measurement request)
    {
        using HttpResponseMessage? result = await _http.PutAsJsonAsync("api/measurement", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        using HttpResponseMessage? result = await _http.DeleteAsync($"api/measurement/{id}");
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

}
