using HelpDesk.Api.Endpoints.Employee;
using Marten;

namespace HelpDesk.Api.Handlers;

public record SubmitterIsVip();

public record SubmitterIsNotVip();

public record RecordVipChecked(Guid id);
public static class VipHandler
{
    public static async Task<RecordVipChecked> Handle(CheckVip command, IDocumentSession session)
    {
        // the code to really check is a vip - go across the network, whatever - pending.
        await Task.Delay(3000); // Todo Get Rid of This.
        session.Events.Append(command.ProblemId, new SubmitterIsVip());
        await session.SaveChangesAsync();
        return new RecordVipChecked(command.ProblemId);
    }
}
