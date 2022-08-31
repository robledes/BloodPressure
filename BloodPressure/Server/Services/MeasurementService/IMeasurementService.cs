namespace BloodPressure.Server.Services.MeasurementService;

public interface IMeasurementService
{
    Task<ServiceResponse<Measurement>> Get(int id);

    Task<ServiceResponse<IEnumerable<Measurement>>> GetAll(int patientId);

    Task<ServiceResponse<bool>> Insert(Measurement measurement);

    Task<ServiceResponse<bool>> Update(Measurement measurement);

    Task<ServiceResponse<bool>> Delete(int id);
}
