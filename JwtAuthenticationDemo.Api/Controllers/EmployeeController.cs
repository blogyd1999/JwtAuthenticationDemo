using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthenticationDemo.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/employees")]
public class EmployeeController : ControllerBase
{
    [HttpGet]
    [ApiVersion("1.0")]
    [Authorize(Policy ="V1Access")]
    public IActionResult GetEmployeesV1()
    {
        return Ok(new
        {
            version = "V1",
            message = "V1 accessible only by Admin"
        });
    }

    [HttpGet]
    [ApiVersion("2.0")]
    [Authorize(Policy ="V2Access")]
    public IActionResult GetEmployeesV2()
    {
        return Ok(new
        {
            version = "V2",
            message = "V2 accessible by Admin and User"
        });
    }
}