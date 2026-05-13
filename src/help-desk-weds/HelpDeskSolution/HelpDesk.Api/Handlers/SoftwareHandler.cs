using HelpDesk.Api.Clients;
using HelpDesk.Api.Endpoints.Employee;
using HelpDesk.Api.ReadModels;
using HelpDesk.Api.Sagas;
using Marten;

namespace HelpDesk.Api.Handlers;

public record SoftwareVerified(string Title, string Manufacturer);

public record SoftwareRetired(DateTimeOffset RetiredDate);

public record SoftwareIsUnknown();


public static class SoftwareHandler
{
    public static async Task<RecordSoftwareCheck> Handle(CheckSoftware command, IDocumentSession session)
    {
        var response = await session.LoadAsync<SoftwareCenterItem>(command.SoftwareId);
        
        var @event = response switch
        {
            null => (object)new SoftwareIsUnknown(),
            { Retired: not null } => new SoftwareRetired(response.Retired.Value),
            { Title: not null, Vendor: not null } => new SoftwareVerified(response.Title, response.Vendor),
            _ => null
        };

        if (@event is not null)
            session.Events.Append(command.ProblemId, @event);

        await session.SaveChangesAsync();
        return new RecordSoftwareCheck(command.ProblemId);
       
       
    }
}