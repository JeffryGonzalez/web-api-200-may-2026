using HelpDesk.Api.Services;
using Marten;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace HelpDesk.Api.Employee;

public enum ProblemImpacts {  Question, Inconvenience, WorkStoppage}
public record ProblemCreate
{
    public required Guid SoftwareId { get; set; }
    [MinLength(10), MaxLength(500)]
    public required string Description { get; set; }

    public required ProblemImpacts Impact { get; set; }
}

public record CreateProblem(Guid Id, EmployeeInfo Employee, ProblemCreate SubmittedProblem);

public static class EmployeeExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEmployeeEndpoints()
        {
            var group = builder.MapGroup("employee");

            group.MapPost("problems", async (ProblemCreate problem, EmployeeSubMapper subMapper, IMessageBus bus) =>
            {
                // It is "valid" 
                var problemId = Guid.NewGuid();
                // write the work to a database, with the ID
                var employeeInfo = await subMapper.GetEmployeeInfoAsync();

                var command = new CreateProblem(problemId, employeeInfo, problem);

                await bus.SendAsync(command); // persist it - write it to the datbase. Try to route this to some code that knows how to deal with this.

                
                return TypedResults.Created($"/employee/{employeeInfo.EmployeeId}/problems/{problemId}");
            });

            group.MapGet("/{employeeId}/problems/{problemId:guid}", async (Guid problemId, IDocumentSession session) =>
            {
                var response = await session.Events.AggregateStreamAsync<Problem>(problemId);
                return TypedResults.Ok(response); // should check for null
            });

            return group;
        }
    }

}

/*
 * POST /employee/problems
Authorization: token identity token from the IDP the WHO IS DOING THIS QUESTION.
Content-Type: application/json

{
   "softwareId": "{guid}",
   "description": "...",
   "impact": "WorkStoppage"
}
*/