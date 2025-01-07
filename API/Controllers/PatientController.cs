using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/patients")]
[ApiController]
public class PatientController(IUserService service) : Controller
{
    // Endpoint: Get all patients 
    [Authorize(Policy = "DoctorPolicy")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllPatients()
    {
        return Ok(await service.GetAllPatientsAsync());
    }
}

