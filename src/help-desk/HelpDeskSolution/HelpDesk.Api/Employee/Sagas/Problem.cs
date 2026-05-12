using JasperFx.Core;
using Marten;
using Wolverine;

namespace HelpDesk.Api.Employee.Sagas;

public record ProblemCreated(ProblemCreateModel Problem);
public record ProblemDeadline(Guid Id) : TimeoutMessage(60.Minutes());
public class Problem : Saga
{
    public Guid Id { get; set; }

    public static async Task< (Problem, ProblemDeadline, CheckSoftware, CheckVip)> 
        Start(CreateProblem problem, ILogger<Problem> logger, IDocumentSession session)
    {

        session.Events.StartStream(problem.Id, new ProblemCreated(problem.SubmittedProblem));
        return (
            new Problem { Id = problem.Id }, 
            new ProblemDeadline(problem.Id), 
            new CheckSoftware(problem.Id, problem.SubmittedProblem.SoftwareId), new CheckVip(problem.Id, problem.Employee.Sub));
    }
}
