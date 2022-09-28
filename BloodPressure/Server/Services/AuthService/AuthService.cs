using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BloodPressure.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IDbConnection _dbConnection;
    private readonly IConfiguration _configuration;

    public AuthService(IDbConnection dbConnection, IConfiguration configuration)
    {
        _dbConnection = dbConnection;
        _configuration = configuration;
    }

    public async Task<ServiceResponse<Patient>> Get(int patientId)
    {
        string sql = "SELECT * FROM Patients " +
                    $"WHERE Patients.Id = {patientId}";

        Patient? patient = await _dbConnection.QueryFirstOrDefaultAsync<Patient>(sql);

        return patient == null ?
            new ServiceResponse<Patient> { Success = false, Message = $"Patient #{patientId} not found. ✘" } :
            new ServiceResponse<Patient> { Success = true, Data = patient };
    }

    public async Task<ServiceResponse<IEnumerable<PatientsList>>> GetAll()
    {
        string sql = "SELECT * FROM Patients " +
                     "ORDER BY Id DESC";

        IEnumerable<PatientsList> patientsList = await _dbConnection.QueryAsync<PatientsList>(sql);

        return patientsList.Any() ?
            new ServiceResponse<IEnumerable<PatientsList>>() { Success = true, Data = patientsList, Message = $"{patientsList.Count()} patient/s were found." } :
            new ServiceResponse<IEnumerable<PatientsList>>() { Success = false, Message = "No patients were found. ✘" };
    }

    public async Task<ServiceResponse<int>> Register(Patient patient, Photo photo, string password)
    {
        string sql = $"SELECT * FROM Patients WHERE IdentificationCard = '{patient.IdentificationCard.ToUpper()}'";

        if (await _dbConnection.QueryFirstOrDefaultAsync<Patient>(sql) != null)
            return new ServiceResponse<int> { Success = false, Message = "Patient already exists ✋" };

        EncryptPassword(password, out byte[] passwordHash, out byte[] passwordSalt);

        sql = "INSERT INTO Patients (IdentificationCard, Firstname, Lastname, PasswordHash, PasswordSalt, GenderCode, BirthDate, Role) " +
              "VALUES (@IdentificationCard, @Firstname, @Lastname, @PasswordHash, @PasswordSalt, @GenderCode, @BirthDate, @Role)";

        int result =await _dbConnection.ExecuteAsync(sql, new
        {
            patient.IdentificationCard,
            patient.Firstname,
            patient.Lastname,
            passwordHash,
            passwordSalt,
            patient.GenderCode,
            patient.BirthDate,
            patient.Role
        });

        sql = "SELECT IDENT_CURRENT('Patients')";
        int patientId = _dbConnection.QueryFirstAsync<int>(sql).Result;

        if (photo.PhotoImage.Length > 0)
            await InsertPhoto(new Photo
            {
                PatientId = patientId,
                PhotoImage = photo.PhotoImage
            });

        return result > 0 ?
            new ServiceResponse<int> { Success = true, Message = $"Patient #{patientId} has been successfully registered. ✓", Data = patient.Id } :
            new ServiceResponse<int> { Success = false, Message = $"Patient #{patientId} could not be registered. ✘" };
    }

    public async Task<ServiceResponse<bool>> Update(Patient patient)
    {
        string sql = "UPDATE Patients " +
                        "SET IdentificationCard = @IdentificationCard, " +
                            "Firstname = @Firstname, " +
                            "Lastname = @Lastname, " +
                            "BirthDate = @BirthDate, " +
                            "GenderCode = @GenderCode, " +
                            "Role = @Role " +
                      "WHERE Id = @Id";

        return await _dbConnection.ExecuteAsync(sql, patient) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Patient #{patient.Id} has been successfully updated. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Patient #{patient.Id} could not be updated. ✘" };
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        string sql = "DELETE FROM Patients " +
                    $"WHERE Id = {id}";

        return await _dbConnection.ExecuteAsync(sql) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Patient #{id} has been successfully deleted. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Patient #{id} could not be deleted. ✘" };
    }

    public async Task<ServiceResponse<string>> Login(string identificationCard, string password)
    {
        string sql = $"SELECT * FROM Patients WHERE IdentificationCard = '{identificationCard.ToUpper()}'";
        Patient? patient = await _dbConnection.QueryFirstOrDefaultAsync<Patient>(sql);

        if (patient == null)
            return new ServiceResponse<string> { Success = false, Message = "Patient not found ✋" };

        using HMACSHA512? hmac = new(patient.PasswordSalt);
        byte[]? computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        if (!computedHash.SequenceEqual(patient.PasswordHash))
            return new ServiceResponse<string> { Success = false, Message = "Wrong password 🔑" };

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
            new Claim(ClaimTypes.UserData, patient.IdentificationCard),
            new Claim(ClaimTypes.Name, patient.FullName),
            new Claim(ClaimTypes.Gender, patient.Gender),
            new Claim(ClaimTypes.Role, patient.Role.ToString())
        };

        SymmetricSecurityKey? key = new(Encoding.UTF8
            .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new ServiceResponse<string> 
        {
            Data = new JwtSecurityTokenHandler().WriteToken(token),
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> ChangePassword(int patientId, string newPassword)
    {
        EncryptPassword(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        string sql = $"UPDATE Patients SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Id = {patientId}";

        return await _dbConnection.ExecuteAsync(sql, new { passwordHash, passwordSalt }) > 0 ?
            new ServiceResponse<bool>
            {
                Success = true,
                Message = $"Password for patient #{patientId} has been changed. ✓"
            } :
            new ServiceResponse<bool>
            {
                Success = false,
                Message = $"Password for patient #{patientId} could not be changed. ✘"
            };
    }

    private static void EncryptPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512? hmac = new();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public async Task<ServiceResponse<Photo>> GetPhoto(int id)
    {
        string sql = "SELECT * FROM Photos " +
                    $"WHERE PatientId = {id}";

        Photo? photo = await _dbConnection.QueryFirstOrDefaultAsync<Photo>(sql);

        return photo == null ?
            new ServiceResponse<Photo> { Success = false, Message = $"Photo for patient #{id} not found. ✘" } : 
            new ServiceResponse<Photo> { Success = true, Message = $"Current photo for patient #{id}. ✓", Data = photo };
    }

    public async Task<ServiceResponse<bool>> InsertPhoto(Photo photo)
    {
        string sql = "INSERT INTO Photos (PatientId, PhotoImage) " +
                     "VALUES (@PatientId, @PhotoImage)";

        return await _dbConnection.ExecuteAsync(sql, new { photo.PatientId, photo.PhotoImage }) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Photo for patient #{photo.PatientId} has been successfully inserted. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Photo for patient #{photo.PatientId} could not be inserted. ✘" };
    }

    public async Task<ServiceResponse<bool>> UpdatePhoto(Photo photo)
    {
        string sql = "UPDATE Photos " +
                        "SET PhotoImage = @PhotoImage " +
                     $"WHERE PatientId = {photo.PatientId}";

        return await _dbConnection.ExecuteAsync(sql, new { photo.PhotoImage }) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Photo for patient #{photo.PatientId} has been successfully updated. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Photo for patient #{photo.PatientId} could not be updated. ✘" };
    }

    public async Task<ServiceResponse<bool>> DeletePhoto(int id)
    {
        string sql = "DELETE FROM Photos " +
                    $"WHERE PatientId = {id}";

        return await _dbConnection.ExecuteAsync(sql) > 0 ?
            new ServiceResponse<bool> { Success = true, Message = $"Photo for patient #{id} has been successfully deleted. ✓" } :
            new ServiceResponse<bool> { Success = false, Message = $"Photo for patient #{id} could not be deleted. ✘" };
    }
}
