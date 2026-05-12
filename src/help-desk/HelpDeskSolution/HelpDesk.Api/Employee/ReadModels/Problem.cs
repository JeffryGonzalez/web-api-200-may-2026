using HelpDesk.Api.Employee.Handlers;
using HelpDesk.Api.Employee.Sagas;
using JasperFx.Events;

namespace HelpDesk.Api.Employee.ReadModels;

public enum ProblemStatus {  Submitted, Checked, AwaitingAssignment, Assigned }
public record EmployeeProblem
{
    public Guid Id { get; init; }
    public int Version { get; init; }
    public required DateTimeOffset ReportedAt { get; init;  }

    public required ProblemCreateModel ReportedIssue { get; init; }
    public ProblemStatus Status { get; init; }

    public static EmployeeProblem Create(IEvent<ProblemCreated> problem)
    {
        return new EmployeeProblem
        {
            Id = problem.Id,
            ReportedAt = problem.Timestamp,
            ReportedIssue = problem.Data.Problem,
            Status = ProblemStatus.Submitted
        };

    }
}

