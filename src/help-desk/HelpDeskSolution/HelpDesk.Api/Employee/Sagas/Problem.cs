using HelpDesk.Api.Employee.Handlers;
using JasperFx.Core;
using Marten;
using Wolverine;

namespace HelpDesk.Api.Employee.Sagas;

public record ProblemCreated(ProblemCreateModel Problem);
public record ProblemDeadline(Guid Id) : TimeoutMessage(60.Minutes());
public class Problem : Saga
{

    // We have ONE hour to check for vip status, to check for software, and when BOTH of those are done, 
    // Say this is ready for Triage (find out if it is a tier1, tier2, 
    public Guid Id { get; set; }

    public bool SoftwareCheckCompleted { get; set; }

    public bool VipCheckCompleted { get; set; }

    public static async Task< (Problem, ProblemDeadline)> 
        Start(CreateProblem problem, ILogger<Problem> logger, IDocumentSession session, IMessageBus bus)
    {
        
        session.Events.StartStream(problem.Id, new ProblemCreated(problem.SubmittedProblem));
        await bus.PublishAsync(new CheckSoftware(problem.Id, problem.SubmittedProblem.SoftwareId));
        await bus.PublishAsync(new CheckVip(problem.Id, problem.Employee.Sub));
        return (
            new Problem { Id = problem.Id }, 
            new ProblemDeadline(problem.Id)
           );
    }

    public void Handle(CheckedSoftware _)
    {
        SoftwareCheckCompleted = true;
    }
}
