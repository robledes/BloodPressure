using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodPressure.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementController : ControllerBase
    {
        private readonly IMeasurementService _measurementService;

        public MeasurementController(IMeasurementService measurementService) => _measurementService = measurementService;

        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<ServiceResponse<Measurement>>> Get(int id)
            => Ok(await _measurementService.Get(id));

        [HttpGet(), Authorize]
        public async Task<ActionResult<ServiceResponse<Measurement>>> GetAll()
            => Ok(await _measurementService.GetAll(GetPatientId()));

        [HttpPost(), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Insert(Measurement request)
        {
            ServiceResponse<bool>? response = await _measurementService.Insert(
                new Measurement
                {
                    PatientId = GetPatientId(),
                    Systolic = request.Systolic,
                    Diastolic = request.Diastolic,
                    Pulse = request.Pulse,
                });

            return Ok(response);
        }

        [HttpPut(), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Update(Measurement request)
        {
            ServiceResponse<bool>? response = await _measurementService.Update(
                new Measurement
                {
                    Id = request.Id,
                    PatientId = request.PatientId,
                    Systolic = request.Systolic,
                    SystolicOB = request.SystolicOB,
                    Diastolic = request.Diastolic,
                    DiastolicOB = request.DiastolicOB,
                    Pulse = request.Pulse,  
                    Date = (DateTime)request.Date
                });

            return Ok(response);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Delete(int id)
            => Ok(await _measurementService.Delete(id));

        private int GetPatientId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
