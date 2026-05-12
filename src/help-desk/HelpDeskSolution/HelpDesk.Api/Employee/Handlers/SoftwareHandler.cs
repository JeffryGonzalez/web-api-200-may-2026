using Marten;

namespace HelpDesk.Api.Employee.Handlers;

public static class SoftwareHandler
{
    public static async Task Handle(CheckSoftware command, IDocumentSession session)
    {
        session.Events.Append(command.ProblemId, new SoftwareVerified("Word", "Microsoft"));
        await session.SaveChangesAsync();
    }
}