using Bumbo.Web.Services.HourRegistration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bumbo.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/HourRegistration")]
public sealed class HourRegistrationApiController : ControllerBase
{
    private readonly IHourRegistrationService _hourRegistrationService;

    public HourRegistrationApiController(IHourRegistrationService hourRegistrationService)
    {
        _hourRegistrationService = hourRegistrationService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("StartStop")]
    public async Task<IActionResult> StartStopTimerForUser(string uuid)
    {
        var result = await _hourRegistrationService.Handle(uuid);

        return result switch
        {
            HourRegistrationResult.ClockedIn => Ok("ClockedIn"),
            HourRegistrationResult.ClockedOut => Ok("ClockedOut"),
            HourRegistrationResult.Error => UnprocessableEntity(),
            HourRegistrationResult.ToSoon => StatusCode(429, "TooSoon"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}