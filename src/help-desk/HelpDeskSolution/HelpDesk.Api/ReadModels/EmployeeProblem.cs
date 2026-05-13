using HelpDesk.Api.Endpoints.Employee;
using HelpDesk.Api.Handlers;
using HelpDesk.Api.Sagas;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace HelpDesk.Api.ReadModels;

public enum ProblemStatus {  Submitted, Checked, AwaitingAssignment, Assigned }

// "A Projection" 
public record EmployeeProblem
{
    public Guid Id { get; init; }
    public int Version { get; init; }
    public string EmployeeId { get; set; } = string.Empty;
    public required DateTimeOffset ReportedAt { get; init;  }

    public required ProblemCreateModel ReportedIssue { get; init; }
    public ProblemStatus Status { get; init; }

    public bool? UnsupportedSoftware { get; init; }

 
}

public class EmployeeProblemProjection : SingleStreamProjection< EmployeeProblem, Guid>
{
    public static EmployeeProblem Create(IEvent<ProblemCreated> problem)
    {
        return new EmployeeProblem
        {
            Id = problem.Id,
            EmployeeId = problem.Data.Employee.EmployeeId,
            ReportedAt = problem.Timestamp,
            ReportedIssue = problem.Data.Problem,
            Status = ProblemStatus.Submitted
        };

    }

    public static EmployeeProblem Apply(SoftwareRetired _, EmployeeProblem current)
    {
        return current with { UnsupportedSoftware = true };
    }
    public static EmployeeProblem Apply(SoftwareIsUnknown _, EmployeeProblem current)
    {
        return current with { UnsupportedSoftware = true };
    }

    public static EmployeeProblem Apply(ProblemVerified _, EmployeeProblem current)
    {
        return current with { Status = ProblemStatus.AwaitingAssignment };
    }
}

