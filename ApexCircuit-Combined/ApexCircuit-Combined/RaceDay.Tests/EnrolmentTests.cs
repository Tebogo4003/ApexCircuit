using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace RaceDay.Tests;

public class EnrolmentTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public EnrolmentTests(TestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task Enrol_AsParticipant_RecordsEnrolment()
    {
        var org = _client;
        await org.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Org", email = "org-e@test.com", password = "Pass123!", role = "Organiser" });
        await org.PostAsJsonAsync("/api/auth/login",
            new { email = "org-e@test.com", password = "Pass123!" });

        var evResp = await org.PostAsJsonAsync("/api/events",
            new { name = "Fun Run", description = "5k fun run",
                  eventDate = DateTime.UtcNow.AddDays(7), location = "Durban",
                  distance = 5m, eventType = "Run" });
        var ev = await evResp.Content.ReadFromJsonAsync<JsonElement>();
        int eventId = ev.GetProperty("eventId").GetInt32();

        var catResp = await org.PostAsJsonAsync($"/api/events/{eventId}/categories",
            new { name = "Open", categoryType = "Age" });
        var cat = await catResp.Content.ReadFromJsonAsync<JsonElement>();
        int catId = cat.GetProperty("categoryId").GetInt32();

        var part = _client;
        await part.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Runner", email = "runner@test.com", password = "Pass123!", role = "Participant" });
        await part.PostAsJsonAsync("/api/auth/login",
            new { email = "runner@test.com", password = "Pass123!" });

        var enrolResp = await part.PostAsJsonAsync("/api/enrolments",
            new { eventId, categoryId = catId });
        enrolResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var mine = await part.GetFromJsonAsync<JsonElement>("/api/enrolments/mine");
        mine.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task Enrol_AsOrganiser_ReturnsForbidden()
    {
        var client = _client;
        await client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "OrgX", email = "orgx@test.com", password = "Pass123!", role = "Organiser" });
        await client.PostAsJsonAsync("/api/auth/login",
            new { email = "orgx@test.com", password = "Pass123!" });

        var response = await client.PostAsJsonAsync("/api/enrolments",
            new { eventId = 1, categoryId = 1 });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Enrol_Duplicate_ReturnsConflict()
    {
        var org = _client;
        await org.PostAsJsonAsync("/api/auth/register",
            new { fullName = "OrgD", email = "orgd@test.com", password = "Pass123!", role = "Organiser" });
        await org.PostAsJsonAsync("/api/auth/login",
            new { email = "orgd@test.com", password = "Pass123!" });

        var evResp = await org.PostAsJsonAsync("/api/events",
            new { name = "Dup Run", description = "duplicate test",
                  eventDate = DateTime.UtcNow.AddDays(7), location = "Cape Town",
                  distance = 10m, eventType = "Run" });
        var ev = await evResp.Content.ReadFromJsonAsync<JsonElement>();
        int eventId = ev.GetProperty("eventId").GetInt32();

        var catResp = await org.PostAsJsonAsync($"/api/events/{eventId}/categories",
            new { name = "Senior", categoryType = "Age" });
        var cat = await catResp.Content.ReadFromJsonAsync<JsonElement>();
        int catId = cat.GetProperty("categoryId").GetInt32();

        var part = _client;
        await part.PostAsJsonAsync("/api/auth/register",
            new { fullName = "RunnerD", email = "runnerd@test.com", password = "Pass123!", role = "Participant" });
        await part.PostAsJsonAsync("/api/auth/login",
            new { email = "runnerd@test.com", password = "Pass123!" });

        await part.PostAsJsonAsync("/api/enrolments", new { eventId, categoryId = catId });
        var second = await part.PostAsJsonAsync("/api/enrolments", new { eventId, categoryId = catId });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
