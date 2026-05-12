using Marten;

namespace HelpDesk.Api.Employee;

public record ProblemCreated();
public record CheckVip(Guid ProblemId, string UserSub);
public record CheckSoftware(Guid ProblemId, Guid SoftwareId);
public static class ProblemHandler
{
    public static async Task<(CheckVip, CheckSoftware)> Handle(CreateProblem problem, IDocumentSession session)
    {
        session.Events.StartStream(problem.Id, new ProblemCreated());
        await session.SaveChangesAsync();
        return (new CheckVip(problem.Id, problem.Employee.Sub), new CheckSoftware(problem.Id, problem.SubmittedProblem.SoftwareId));
    }
}

public record SubmitterIsVip();

public record SubmitterIsNotVip();


public record SoftwareVerified(string Title, string Manufacturer);

public record SoftwareRetired(DateTimeOffset RetiredDate);

public record SoftwareIsUnknown();
