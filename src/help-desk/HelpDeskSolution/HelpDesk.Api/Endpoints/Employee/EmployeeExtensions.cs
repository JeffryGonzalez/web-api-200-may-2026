using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Services;
using Wolverine;

namespace HelpDesk.Api.Endpoints.Employee;

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

            group.MapPost("problems", async (ProblemCreateModel problem, IMapEmployeeSubsToInternalIds subMapper, IMessageBus bus) =>
            {
                // It is "valid" 
                var problemId = Guid.NewGuid();
                // write the work to a database, with the ID
                var employeeInfo = await subMapper.GetEmployeeInfoAsync();

                var command = new CreateProblem(problemId, employeeInfo, problem);

                await bus.SendAsync(command); // persist it - write it to the database. Try to route this to some code that knows how to deal with this.

                
                return TypedResults.Created($"/employees/{employeeInfo.EmployeeId}/problems/{problemId}");
            });
            
            return group;
        }
    }

}

