using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodPressure.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpGet(), Authorize]
        public async Task<ActionResult<ServiceResponse<Patient>>> Get()
            => Ok(await _authService.Get(GetPatientId()));

        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<ServiceResponse<Patient>>> Get(int id)
            => Ok(await _authService.Get(id));

        [HttpGet("[action]"), Authorize]
        public async Task<ActionResult<ServiceResponse<IEnumerable<PatientsList>>>> GetAll()
            => Ok(await _authService.GetAll());

        [HttpGet("[action]"), Authorize]
        public async Task<ActionResult<ServiceResponse<Photo>>> GetPhoto()
            => Ok(await _authService.GetPhoto(GetPatientId()));

        [HttpGet("[action]/{id}"), Authorize]
        public async Task<ActionResult<ServiceResponse<Photo>>> GetPhoto(int id)
            => Ok(await _authService.GetPhoto(id));

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister request)
        {
            ServiceResponse<int>? response = await _authService.Register(
                new Patient
                {
                    IdentificationCard = request.IdentificationCard,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    GenderCode = request.GenderCode,
                    BirthDate = (DateTime)request.BirthDate,
                },
                new Photo
                {
                    PhotoImage = request.Photo
                },
                request.Password);

            return Ok(response);
        }

        [HttpPost("[action]"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> InsertPhoto(Photo request)
        {
            ServiceResponse<bool>? response = await _authService.InsertPhoto(
                new Photo
                {
                    PatientId = GetPatientId(),
                    PhotoImage = request.PhotoImage
                });

            return Ok(response);
        }

        [HttpPut(), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Update(Patient request)
        {
            ServiceResponse<bool>? response = await _authService.Update(
                new Patient
                {
                    Id = request.Id > 0 ? request.Id : GetPatientId(),
                    IdentificationCard = request.IdentificationCard,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    GenderCode = request.GenderCode,
                    BirthDate = (DateTime)request.BirthDate,
                    Role = request.Role
                });
            
            return Ok(response);
        }

        [HttpPut("[action]"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdatePhoto(Photo request)
        {
            ServiceResponse<bool>? response = await _authService.UpdatePhoto(
                new Photo
                {
                    PatientId = GetPatientId(),
                    PhotoImage = request.PhotoImage
                });

            return Ok(response);
        }

        [HttpDelete(), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Delete()
            => Ok(await _authService.Delete(GetPatientId()));

        [HttpDelete("[action]"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> DeletePhoto() 
            => Ok(await _authService.DeletePhoto(GetPatientId()));

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLogin request)
            => Ok(await _authService.Login(request.IdentificationCard, request.Password));

        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string newPassword)
            => Ok(await _authService.ChangePassword(GetPatientId(), newPassword));

        private int GetPatientId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
