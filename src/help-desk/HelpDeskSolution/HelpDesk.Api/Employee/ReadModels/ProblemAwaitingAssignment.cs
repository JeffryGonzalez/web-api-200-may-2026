using HelpDesk.Api.Employee.Handlers;
using JasperFx.Events;

namespace HelpDesk.Api.Employee.ReadModels;

public record ProblemVerified(Guid Id);



public record ProblemAwaitingAssignment
{
    public Guid Id { get; init; }
    public int Version { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public TimeSpan Active
    {
        get
        {
            return DateTimeOffset.UtcNow - CreatedAt;
        }
    }

    public SoftwareReference? SoftwareReference { get; init; } = null;
    public RetiredSoftwareReference? RetiredSoftwareReference { get; init; } = null;
    public bool? UnlistedSoftware { get; set; }
    public bool IsVip { get; init; } = false;

    public static ProblemAwaitingAssignment Create(IEvent<ProblemVerified> evt)
    {
        return new ProblemAwaitingAssignment
        {
            Id = evt.Data.Id,
            CreatedAt = evt.Timestamp,
        };
        
    }

    public ProblemAwaitingAssignment Apply(SubmitterIsVip _, ProblemAwaitingAssignment current)
    {
        return current with { IsVip = true };
    }
    public ProblemAwaitingAssignment Apply(SubmitterIsNotVip _, ProblemAwaitingAssignment current)
    {
        return current with { IsVip = false };
    }

    public ProblemAwaitingAssignment Apply(SoftwareIsUnknown evt, ProblemAwaitingAssignment current)
    {
        return current with {UnlistedSoftware = true};
    }

    public ProblemAwaitingAssignment Apply(SoftwareVerified evt, ProblemAwaitingAssignment current)
    {
        return current with {SoftwareReference = new SoftwareReference()
        {
            Title = evt.Title,
            Vendor = evt.Manufacturer
        }};
    }
    public ProblemAwaitingAssignment Apply(SoftwareRetired evt, ProblemAwaitingAssignment current)
    {
        return current with { RetiredSoftwareReference = new RetiredSoftwareReference()
        {
            RetiredOn = evt.RetiredDate
        }};
    }

}

public record SoftwareReference
{
    
    public required string Title { get; init; }
    public required string Vendor { get; init; }
}

public record RetiredSoftwareReference
{
    public DateTimeOffset RetiredOn { get; init; }
   
}