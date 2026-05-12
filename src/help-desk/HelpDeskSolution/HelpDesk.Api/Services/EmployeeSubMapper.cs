namespace HelpDesk.Api.Services;

public class EmployeeSubMapper
{
    // todo: HttpContext Accessor, get the sub from the claims, etc.
    public Task<EmployeeInfo> GetEmployeeInfoAsync(CancellationToken token = default)
    {
        return Task.FromResult(new EmployeeInfo("sue@company.com", "x0039"));
    }
}

public record EmployeeInfo(string Sub, string EmployeeId);