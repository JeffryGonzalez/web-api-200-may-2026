using System;
using System.Collections.Generic;
using System.Text;
using WireMock.Client.Builders;

namespace AppHost;

public static class SoftwareApiMock
{
    public static Task Build(AdminApiMappingBuilder builder)
    {
        builder.Given(b => b.WithRequest(req => req.UsingGet().WithPath("/help-desk/catalog/25ff5f2d-cdaa-4a6b-96f7-f018d3f27e59"))
        .WithResponse(res => res.WithBodyAsJson(new { title = "Destiny", vendor = "Bungie" })));
        return Task.CompletedTask;

    }
}