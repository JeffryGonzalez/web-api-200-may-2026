using HelpDesk.Api.Clients;
using Marten;

namespace HelpDesk.Api.Employee.Handlers;

public record SoftwareVerified(string Title, string Manufacturer);

public record SoftwareRetired(DateTimeOffset RetiredDate);

public record SoftwareIsUnknown();

public record CheckedSoftware();
public static class SoftwareHandler
{
    public static async Task<CheckedSoftware> Handle(CheckSoftware command, IDocumentSession session, SoftwareCenterHttpClient client)
    {
        var response = await client.CheckForSoftwareAvailabilityAsync(command.SoftwareId);
        if(response is null)
        {
            session.Events.Append(command.ProblemId, new SoftwareIsUnknown());
            await session.SaveChangesAsync();
           
        }
        if(response.RetiredDate is not null)
        {
            session.Events.Append(command.ProblemId, new SoftwareRetired(response.RetiredDate.Value));
            await session.SaveChangesAsync();
           

        } else
        {
            session.Events.Append(command.ProblemId, new SoftwareVerified(response.Title, response.Vendor));
            await session.SaveChangesAsync();
        }

        return new CheckedSoftware();
       
       
    }
}