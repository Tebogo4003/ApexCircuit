using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace RaceDay.Tests;

public class ProfileTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public ProfileTests(TestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetProfile_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/profile");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_Authenticated_ReturnsOk()
    {
        await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Profile User", email = "profile@test.com",
                  password = "Pass123!", role = "Participant" });
        await _client.PostAsJsonAsync("/api/auth/login",
            new { email = "profile@test.com", password = "Pass123!" });

        var response = await _client.GetAsync("/api/profile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_ChangesName_ReturnsOk()
    {
        await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Old Name", email = "updatetest@test.com",
                  password = "Pass123!", role = "Organiser" });
        await _client.PostAsJsonAsync("/api/auth/login",
            new { email = "updatetest@test.com", password = "Pass123!" });

        var response = await _client.PutAsJsonAsync("/api/profile",
            new { fullName = "New Name", email = "updatetest@test.com" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
