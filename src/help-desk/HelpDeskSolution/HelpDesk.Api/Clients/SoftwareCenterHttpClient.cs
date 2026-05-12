namespace HelpDesk.Api.Clients;

public class SoftwareCenterHttpClient(HttpClient client)
{

    public async Task<SoftwareCheckResponse?> CheckForSoftwareAvailabilityAsync(Guid softwareId)
    {
        var response = await client.GetAsync($"/help-desk/catalog/{softwareId}");

        //response.EnsureSuccessStatusCode();
        // if it is 404, what do we do?
        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode(); // 404 already returned, any non-success (200-299) punch me in the nose. 

        var responseBody = await response.Content.ReadFromJsonAsync<SoftwareCheckResponse>();

        if (responseBody != null)
        {
          return responseBody;
        }

        return null;

    }

}

public record SoftwareCheckResponse
{
    public string? Title { get; set; }
    public string? Vendor { get; set; }
    public DateTimeOffset? RetiredDate { get; set; }
}