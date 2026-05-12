using Marten;

namespace HelpDesk.Api.Employee.Handlers;

public record SubmitterIsVip();

public record SubmitterIsNotVip();

public static class VipHandler
{
    public static async Task Handle(CheckVip command, IDocumentSession session)
    {
        // the code to really check is a vip - go across the network, whatever - pending.
        session.Events.Append(command.ProblemId, new SubmitterIsVip());
        await session.SaveChangesAsync();
    }
}
