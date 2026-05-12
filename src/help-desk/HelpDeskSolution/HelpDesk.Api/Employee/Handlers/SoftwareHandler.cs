using Marten;

namespace HelpDesk.Api.Employee.Handlers;

public record SoftwareVerified(string Title, string Manufacturer);

public record SoftwareRetired(DateTimeOffset RetiredDate);

public record SoftwareIsUnknown();
public static class SoftwareHandler
{
    public static async Task Handle(CheckSoftware command, IDocumentSession session)
    {
        session.Events.Append(command.ProblemId, new SoftwareVerified("Word", "Microsoft"));
        await session.SaveChangesAsync();
    }
}