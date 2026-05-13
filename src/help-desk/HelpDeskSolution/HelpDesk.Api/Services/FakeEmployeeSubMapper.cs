using System.Security.Cryptography;
using System.Text;

namespace HelpDesk.Api.Services;

public class FakeEmployeeSubMapper(IHttpContextAccessor httpContextAccessor, ILogger<FakeEmployeeSubMapper> logger) : IMapEmployeeSubsToInternalIds
{
    // todo: This is a FAKE AUTH thing for class. Replace with the provider in the style in SoftwareCenter
    // todo: NOT SECURE
    public Task<EmployeeInfo> GetEmployeeInfoAsync(CancellationToken token = default)
    {
        var user = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString() ?? "anonymous";
        var userId = CreateMd5(user);
        logger.LogWarning("Using the FakeEmployeeSubMapper for user {User} with sub {Sub}", user, userId);
        return Task.FromResult(new EmployeeInfo(user, userId));
    }

    private static string CreateMd5(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}

public record EmployeeInfo(string Sub, string EmployeeId);