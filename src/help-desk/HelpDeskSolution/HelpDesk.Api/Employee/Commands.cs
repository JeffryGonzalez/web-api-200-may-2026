using HelpDesk.Api.Services;

namespace HelpDesk.Api.Employee;

public record CreateProblem(Guid Id, EmployeeInfo Employee, ProblemCreateModel SubmittedProblem);
public record CheckVip(Guid ProblemId, string UserSub);
public record CheckSoftware(Guid ProblemId, Guid SoftwareId);
