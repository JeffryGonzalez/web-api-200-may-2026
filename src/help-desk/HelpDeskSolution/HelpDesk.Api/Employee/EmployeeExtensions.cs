using HelpDesk.Api.Employee.ReadModels;
using HelpDesk.Api.Employee.Sagas;
using HelpDesk.Api.Services;
using Marten;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace HelpDesk.Api.Employee;

public enum ProblemImpacts {  Question, Inconvenience, WorkStoppage}
public record ProblemCreateModel
{
    public required Guid SoftwareId { get; set; }
    [MinLength(10), MaxLength(500)]
    public required string Description { get; set; }

    public required ProblemImpacts Impact { get; set; }
}



public static class EmployeeExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEmployeeEndpoints()
        {
            var group = builder.MapGroup("employee");

            group.MapPost("problems", async (ProblemCreateModel problem, EmployeeSubMapper subMapper, IMessageBus bus) =>
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
                // TODO = What about that unused employeeId parameter?
                var response = await session.Events.AggregateStreamAsync<EmployeeProblem>(problemId);
                return response;
            });
            group.MapGet("sagas", async (IDocumentSession session) =>
            {
                var response = await session.Query<Problem>().ToListAsync();
                return response;
            
            });
            group.MapGet("triaged/{id:guid}", async (Guid id, IDocumentSession session) => await session.Events.AggregateStreamAsync<ProblemAwaitingAssignment>(id));

            return group;
        }
    }

}

