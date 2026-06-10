using Microsoft.AspNetCore.Mvc;
using test_A.DTO.Patients.Post;
using test_A.Services;

namespace test_A.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? lastName, CancellationToken cancellationToken)
    {
        var patients = await _patientService.GetPatientsAsync(lastName, cancellationToken);
        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var newId = await _patientService.CreatePatientWithAppointmentAsync(request, cancellationToken);
            HttpContext.Response.Headers.Location = $"/api/patients/{newId}";
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
