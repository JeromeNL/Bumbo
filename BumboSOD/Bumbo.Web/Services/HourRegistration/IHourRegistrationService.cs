namespace Bumbo.Web.Services.HourRegistration;

public interface IHourRegistrationService
{
    public Task<HourRegistrationResult> Handle(string uuid);
}