using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace RaceDay.Tests;

public class EventTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public EventTests(TestFixture fixture) => _client = fixture.Client;

    private async Task<HttpClient> GetAuthenticatedClient(string role, string email)
    {
        var client = _client;
        await client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Test User", email, password = "Pass123!", role });
        await client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "Pass123!" });
        return client;
    }

    [Fact]
    public async Task CreateEvent_AsOrganiser_ReturnsCreated()
    {
        var client = await GetAuthenticatedClient("Organiser", "org1@test.com");
        var response = await client.PostAsJsonAsync("/api/events",
            new { name = "City Marathon", description = "Annual city marathon",
                  eventDate = DateTime.UtcNow.AddDays(30), location = "Cape Town",
                  distance = 42.2m, eventType = "Run" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateEvent_AsParticipant_ReturnsForbidden()
    {
        var client = await GetAuthenticatedClient("Participant", "part1@test.com");
        var response = await client.PostAsJsonAsync("/api/events",
            new { name = "Hack Event", description = "x", eventDate = DateTime.UtcNow,
                  location = "JHB", distance = 5m, eventType = "Run" });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEvents_Unauthenticated_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/events");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateEvent_AsNonOwner_ReturnsForbidden()
    {
        var org1 = await GetAuthenticatedClient("Organiser", "org2@test.com");
        var org2 = await GetAuthenticatedClient("Organiser", "org3@test.com");

        var createResp = await org1.PostAsJsonAsync("/api/events",
            new { name = "Trail Run", description = "Mountain trail",
                  eventDate = DateTime.UtcNow.AddDays(10), location = "Stellenbosch",
                  distance = 21m, eventType = "Run" });
        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        int eventId = created.GetProperty("eventId").GetInt32();

        var response = await org2.PutAsJsonAsync($"/api/events/{eventId}",
            new { name = "Hijacked", description = "x", eventDate = DateTime.UtcNow,
                  location = "x", distance = 1m, eventType = "Walk" });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteEvent_AsOwner_ReturnsNoContent()
    {
        var org = await GetAuthenticatedClient("Organiser", "org4@test.com");

        var createResp = await org.PostAsJsonAsync("/api/events",
            new { name = "Temp Event", description = "to delete",
                  eventDate = DateTime.UtcNow.AddDays(5), location = "Pretoria",
                  distance = 10m, eventType = "Cycle" });
        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        int eventId = created.GetProperty("eventId").GetInt32();

        var response = await org.DeleteAsync($"/api/events/{eventId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
