namespace HelpDesk.Api.Services;

public interface IMapEmployeeSubsToInternalIds
{
    Task<EmployeeInfo> GetEmployeeInfoAsync(CancellationToken token = default);
}