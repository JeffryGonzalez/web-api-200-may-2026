using Marten;
using SharedTypes;

namespace HelpDesk.Api.Handlers;

public record SoftwareCenterItem(Guid Id, string? Title, string? Vendor, DateTimeOffset? RetiredDate = null);


public class SoftwareCenterHandler
{
    public async Task Handle(AddSoftwareItem item, IDocumentSession session)
    {
        var newitem = new SoftwareCenterItem(item.Id, item.Title, item.Vendor);
        session.Store(newitem);
        await session.SaveChangesAsync();
    }

    public async Task Handle(RetireSoftwareItem item, IDocumentSession session)
    {
        var savedItem = await session.LoadAsync<SoftwareCenterItem>(item.Id);
        if(savedItem is null)
        {
            throw new Exception("what??");
        }
        var updatedItem = savedItem with { RetiredDate = item.RetiredAt, Title = null, Vendor = null };
        session.Store(updatedItem);
        await session.SaveChangesAsync();
    }
}
