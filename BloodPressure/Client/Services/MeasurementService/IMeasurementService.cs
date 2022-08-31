namespace BloodPressure.Client.Services.MeasurementService;

public interface IMeasurementService
{
    Task<ServiceResponse<Measurement>> Get(int id);

    Task<ServiceResponse<IEnumerable<Measurement>>> GetAll();

    Task<ServiceResponse<bool>> Insert(Measurement request);
    
    Task<ServiceResponse<bool>> Update(Measurement request);

    Task<ServiceResponse<bool>> Delete(int id);
}
