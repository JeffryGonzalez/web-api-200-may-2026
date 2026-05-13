using HelpDesk.Api.ReadModels;
using HelpDesk.Api.Sagas;
using Marten;

namespace HelpDesk.Api.Endpoints.Techs;

public static class TechsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapTechEndpoints()
        {
            var group = builder.MapGroup("techs");

          
            group.MapGet("problems-awaiting-triage", async (IDocumentSession session) =>
            {
                var response = await session.Query<Problem>().ToListAsync();
                return response;
            
            });
            
            group.MapGet("triaged", async (IDocumentSession session) => await session.Query<TriagedProblem>().ToListAsync() );
            group.MapGet("triaged/{id:guid}", async (Guid id, IDocumentSession session) => await session.Events.AggregateStreamAsync<ProblemAwaitingAssignment>(id));

            return group;
        }
    }
}