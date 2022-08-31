namespace BloodPressure.Client.Services.AuthService;

public interface IAuthService
{
    Task<ServiceResponse<Patient>> Get();

    Task<ServiceResponse<Patient>> Get(int id);

    Task<ServiceResponse<IEnumerable<PatientsList>>> GetAll();

    Task<ServiceResponse<Photo>> GetPhoto();

    Task<ServiceResponse<Photo>> GetPhoto(int id);

    Task<ServiceResponse<int>> Register(UserRegister request);

    Task<ServiceResponse<bool>> InsertPhoto(Photo request);

    Task<ServiceResponse<bool>> Update(Patient request);

    Task<ServiceResponse<bool>> UpdatePhoto(Photo request);

    Task<ServiceResponse<bool>> Delete();

    Task<ServiceResponse<bool>> DeletePhoto();

    Task<ServiceResponse<string>> Login(UserLogin request);

    Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request);
}
