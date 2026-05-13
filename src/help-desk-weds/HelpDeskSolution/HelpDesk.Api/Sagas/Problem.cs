using HelpDesk.Api.Endpoints.Employee;
using HelpDesk.Api.Handlers;
using HelpDesk.Api.ReadModels;
using HelpDesk.Api.Services;
using Marten;
using Wolverine;

namespace HelpDesk.Api.Sagas;

public record ProblemCreated(ProblemCreateModel Problem, EmployeeInfo Employee);
public record RecordSoftwareCheck(Guid Id);
public class Problem : Saga
{
    public Guid Id { get; set; }

    public bool SoftwareCheckCompleted { get; set; }

    public bool VipCheckCompleted { get; set; }

    public static async Task<Problem> Start(CreateProblem problem, IMessageBus bus, IDocumentSession session)
    {
        session.Events.StartStream(problem.Id, new ProblemCreated(problem.SubmittedProblem, problem.Employee));
        await bus.PublishAsync(new CheckSoftware(problem.Id, problem.SubmittedProblem.SoftwareId));
        await bus.PublishAsync(new CheckVip(problem.Id, problem.Employee.Sub));
        return new Problem { Id = problem.Id };
    }

    public async ValueTask Handle(RecordSoftwareCheck _, IDocumentSession session, IMessageBus bus)
    {
        SoftwareCheckCompleted = true;
        await Verify(session, bus);
    }

    public async ValueTask Handle(RecordVipChecked _, IDocumentSession session, IMessageBus bus)
    {
        VipCheckCompleted = true;
        await Verify(session, bus);
    }

    private async ValueTask Verify(IDocumentSession session, IMessageBus bus)
    {
        if (SoftwareCheckCompleted && VipCheckCompleted)
        {
            session.Events.Append(Id, new ProblemVerified(Id));
            await session.SaveChangesAsync();
            MarkCompleted(); // is a Saga method - means we are done, delete this sucker.
            await bus.PublishAsync(new StartTriage(Id));
        }
    }
}