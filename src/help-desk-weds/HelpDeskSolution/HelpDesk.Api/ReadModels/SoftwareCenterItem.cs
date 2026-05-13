using HelpDesk.Api.Handlers;
using Marten.Events.Aggregation;

namespace HelpDesk.Api.ReadModels;

public record SoftwareCenterItem
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Vendor { get; set;  }  = string.Empty;

    public DateTimeOffset? Retired { get; set; }

}

public class SoftwareCenterItemProjection : SingleStreamProjection<SoftwareCenterItem, Guid>
{
    public SoftwareCenterItemProjection()
    {
        //DeleteEvent<SoftwareItemRetired>();
    }
    public static SoftwareCenterItem Create(SoftwareItemAdded e)
    {
        return new SoftwareCenterItem
        {
            Id = e.Id,
            Title = e.Title,
            Vendor = e.Vendor,
           
        };
    }

    public static SoftwareCenterItem Apply(SoftwareItemRetired e, SoftwareCenterItem current)
    {
        return current with { Retired = e.RetiredOn, Title = null, Vendor = null };
    }
}