namespace BloodPressure.Server.Services.MeasurementService;

public class MeasurementService : IMeasurementService
{
    private readonly IDbConnection _dbConnection;

    public MeasurementService(IDbConnection dbConnection) => _dbConnection = dbConnection;

    public async Task<ServiceResponse<Measurement>> Get(int id)
    {
        string sql = "SELECT * FROM Measurements " +
                    $"WHERE Id = {id}";

        Measurement? measurement = await _dbConnection.QueryFirstOrDefaultAsync<Measurement>(sql);

        return measurement == null ?
            new ServiceResponse<Measurement> { Success = false, Message = $"Measurement #{id} not found. ✘" } :
            new ServiceResponse<Measurement> { Success = true, Data = measurement };
    }

    public async Task<ServiceResponse<IEnumerable<Measurement>>> GetAll(int patientId)
    {
        string sql = "SELECT * FROM Measurements " +
                    $"WHERE PatientId = {patientId}" +
                     "ORDER BY Id DESC";

        IEnumerable<Measurement> measurements = await _dbConnection.QueryAsync<Measurement>(sql);

        return measurements.Any() ?
            new ServiceResponse<IEnumerable<Measurement>>() { Success = true, Data = measurements, Message = $"{measurements.Count()} measurement/s were found." } :
            new ServiceResponse<IEnumerable<Measurement>>() { Success = false, Message = "No measurements were found. ✘" };
    }

    public async Task<ServiceResponse<bool>> Insert(Measurement measurement)
    {
        string sql = "SELECT COUNT(*) " +
                       "FROM Measurements " +
                     $"WHERE PatientId = {measurement.PatientId}";

        int total = await _dbConnection.QuerySingleAsync<int>(sql);

        if (total == 15)
        {
            sql = "SELECT Id " +
                    "FROM Measurements " +
                  $"WHERE PatientId = {measurement.PatientId}";

            int id = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql);

            sql = "DELETE FROM Measurements " +
                       $"WHERE Id = {id}";

            await _dbConnection.ExecuteAsync(sql);
        }

        measurement = await CheckLimits(measurement);

        sql = "INSERT INTO Measurements (PatientId, Systolic, SystolicOB, Diastolic, DiastolicOB, Pulse, Date) " +
                   "VALUES (@PatientId, @Systolic, @SystolicOB, @Diastolic, @DiastolicOB, @Pulse, @Date)";

        return await _dbConnection.ExecuteAsync(sql, new
        {
            measurement.PatientId,
            measurement.Systolic,
            measurement.SystolicOB,
            measurement.Diastolic,
            measurement.DiastolicOB,
            measurement.Pulse,
            measurement.Date
        }) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Measurement for patient #{measurement.PatientId} has been successfully inserted. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Measurement for patient #{measurement.PatientId} could not be inserted. ✘" };
    }

    public async Task<ServiceResponse<bool>> Update(Measurement measurement)
    {
        measurement = await CheckLimits(measurement);

        string sql = "UPDATE Measurements " +
                        "SET Systolic = @Systolic, " +
                            "SystolicOB = @SystolicOB, " +
                            "Diastolic = @Diastolic, " +
                            "DiastolicOB = @DiastolicOB, " +
                            "Pulse = @Pulse " +
                      "WHERE Id = @Id";

        return await _dbConnection.ExecuteAsync(sql, measurement) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Measurement #{measurement.Id} has been successfully updated. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Measurement #{measurement.Id} could not be updated. ✘" };
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        string sql = "DELETE FROM Measurements " +
                          $"WHERE Id = {id}";

        return await _dbConnection.ExecuteAsync(sql) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Measurement has been successfully deleted. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Measurement could not be deleted. ✘" };
    }

    private async Task<Measurement> CheckLimits(Measurement measurement)
    {
        string sql = "SELECT * FROM Patients " +
                    $"WHERE Patients.Id = {measurement.PatientId}";

        Patient? patient = await _dbConnection.QueryFirstOrDefaultAsync<Patient>(sql);

        sql = "SELECT * FROM Limits " +
             $"WHERE AgeFrom <= {patient.Age} AND AgeTo >= {patient.Age}";

        Limit? limit = await _dbConnection.QueryFirstOrDefaultAsync<Limit>(sql);

        if (measurement.Systolic < limit.SystolicMin)
            measurement.SystolicOB = "▼";
        else if (measurement.Systolic > limit.SystolicMax)
            measurement.SystolicOB = "▲";
        else
            measurement.SystolicOB = null;

        if (measurement.Diastolic < limit.DiastolicMin)
            measurement.DiastolicOB = "▼";
        else if (measurement.Diastolic > limit.DiastolicMax)
            measurement.DiastolicOB = "▲";
        else
            measurement.DiastolicOB = null;

        return measurement;
    }
}
