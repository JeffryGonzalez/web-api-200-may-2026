using Marten;
using SharedTypes;

namespace HelpDesk.Api.Handlers;

//public record SoftwareCenterItem(Guid Id, string? Title, string? Vendor, DateTimeOffset? RetiredDate = null);


public record SoftwareItemAdded(Guid Id, string Title, string Vendor);
public record SoftwareItemRetired(Guid Id, DateTimeOffset RetiredOn);
public class SoftwareCenterHandler
{
    public async Task Handle(AddSoftwareItem item, IDocumentSession session)
    {
        var evt = new SoftwareItemAdded(item.Id, item.Title, item.Vendor);
        session.Events.Append(evt.Id, evt);
       
        await session.SaveChangesAsync();
    }

    public async Task Handle(RetireSoftwareItem item, IDocumentSession session)
    {
        var evt = new SoftwareItemRetired(item.Id, item.RetiredAt);
        session.Events.Append(evt.Id, evt);
        await session.SaveChangesAsync();
    }
}
