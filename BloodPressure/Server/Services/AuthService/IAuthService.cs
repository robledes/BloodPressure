namespace BloodPressure.Server.Services.AuthService;

public interface IAuthService
{
    Task<ServiceResponse<Patient>> Get(int patientId);

    Task<ServiceResponse<IEnumerable<PatientsList>>> GetAll();
    
    Task<ServiceResponse<int>> Register(Patient patient, Photo photo, string password);

    Task<ServiceResponse<bool>> Update(Patient patient);

    Task<ServiceResponse<bool>> Delete(int id);

    Task<ServiceResponse<string>> Login(string identificationCard, string password);

    Task<ServiceResponse<bool>> ChangePassword(int patientId, string newPassword);

    Task<ServiceResponse<Photo>> GetPhoto(int id);

    Task<ServiceResponse<bool>> InsertPhoto(Photo photo);

    Task<ServiceResponse<bool>> UpdatePhoto(Photo photo);

    Task<ServiceResponse<bool>> DeletePhoto(int id);
}
