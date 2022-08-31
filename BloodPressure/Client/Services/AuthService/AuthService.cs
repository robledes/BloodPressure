using System.Net.Http.Json;

namespace BloodPressure.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http) => _http = http;

    public async Task<ServiceResponse<Patient>> Get()
        => await _http.GetFromJsonAsync<ServiceResponse<Patient>>("api/auth");

    public async Task<ServiceResponse<Patient>> Get(int id)
        => await _http.GetFromJsonAsync<ServiceResponse<Patient>>($"api/auth/{id}");

    public async Task<ServiceResponse<IEnumerable<PatientsList>>> GetAll()
        => await _http.GetFromJsonAsync<ServiceResponse<IEnumerable<PatientsList>>>("api/auth/getall");

    public async Task<ServiceResponse<Photo>> GetPhoto()
        => await _http.GetFromJsonAsync<ServiceResponse<Photo>>("api/auth/getphoto");

    public async Task<ServiceResponse<Photo>> GetPhoto(int id)
        => await _http.GetFromJsonAsync<ServiceResponse<Photo>>($"api/auth/getphoto/{id}");

    public async Task<ServiceResponse<int>> Register(UserRegister request)
    {
        using HttpResponseMessage? result = await _http.PostAsJsonAsync("api/auth/register", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
    }

    public async Task<ServiceResponse<bool>> InsertPhoto(Photo request)
    {
        using HttpResponseMessage? result = await _http.PostAsJsonAsync("api/auth/insertphoto", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> Update(Patient request)
    {
        using HttpResponseMessage? result = await _http.PutAsJsonAsync("api/auth", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> UpdatePhoto(Photo request)
    {
        using HttpResponseMessage? result = await _http.PutAsJsonAsync("api/auth/updatephoto", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> Delete()
    {
        using HttpResponseMessage? result = await _http.DeleteAsync("api/auth");
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<bool>> DeletePhoto()
    {
        using HttpResponseMessage? result = await _http.DeleteAsync("api/auth/deletephoto");
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }

    public async Task<ServiceResponse<string>> Login(UserLogin request)
    {
        using HttpResponseMessage? result = await _http.PostAsJsonAsync("api/auth/login", request);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
    }

    public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
    {
        using HttpResponseMessage? result = await _http.PostAsJsonAsync("api/auth/change-password", request.Password);
        return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
    }
}
