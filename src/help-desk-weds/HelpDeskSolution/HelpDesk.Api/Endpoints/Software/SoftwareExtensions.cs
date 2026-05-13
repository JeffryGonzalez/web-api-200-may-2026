using HelpDesk.Api.Handlers;
using HelpDesk.Api.ReadModels;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Endpoints.Software;

public static class SoftwareExtensions
{
    extension(IEndpointRouteBuilder routes)
    {
        public IEndpointRouteBuilder MapSoftware()
        {
            var group = routes.MapGroup("software");
            group.MapGet("", async (IDocumentSession session) =>
            {
                var response = await session.Query<SoftwareCenterItem>()
               // .Where(s => s.Retired == null)
                .ToListAsync();
                return Results.Ok(response);
            });

            group.MapGet("/{id:guid}", async (Guid id, [FromQuery] long? version, IDocumentSession session) =>
            {
                if(version is not null)
                {
                    var result = await session.Events.AggregateStreamAsync<SoftwareCenterItem>(id, version.Value);
                    return Results.Ok(result);
                } else
                {
                    var result = await session.Events.AggregateStreamAsync<SoftwareCenterItem>(id);
                    return Results.Ok(result);

                }
            });
            return group;
        }


    }
}
