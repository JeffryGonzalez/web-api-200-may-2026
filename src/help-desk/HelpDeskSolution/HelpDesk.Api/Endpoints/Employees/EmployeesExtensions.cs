using HelpDesk.Api.ReadModels;
using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.Endpoints.Employees;

public static  class EmployeesExtensions
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public  IEndpointRouteBuilder MapEmployeesEndpoints()
        {
            var group = endpoints.MapGroup("employees");

            group.MapGet("/{employeeId}/problems/{problemId:guid}", async (Guid problemId, IDocumentSession session, IMapEmployeeSubsToInternalIds employeeMapper) =>
            {
                var emp = await employeeMapper.GetEmployeeInfoAsync();
                
                var response = await session.Query<EmployeeProblem>()
                    .Where(e => e.Id == problemId ).SingleOrDefaultAsync();
             
                if (response is not null && response.EmployeeId != emp.EmployeeId)
                {
                    return Results.Unauthorized();
                }

                if (response is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(response);
            });

            group.MapGet("/{employeeId}/problems",
                async (string employeeId, IDocumentSession session, IMapEmployeeSubsToInternalIds employeeMapper) =>
                {
                    var empInfo =  await employeeMapper.GetEmployeeInfoAsync();
                    if ( empInfo.EmployeeId != employeeId)
                    {
                        return Results.Unauthorized();
                    }
                    var results = await session.Query<EmployeeProblem>().Where(e => e.EmployeeId == employeeId).ToListAsync();
                    return Results.Ok(results);
                });
            return group;
        }
    }
}