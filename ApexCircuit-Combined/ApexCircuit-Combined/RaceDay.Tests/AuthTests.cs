using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace RaceDay.Tests;

public class AuthTests : IClassFixture<TestFixture>
{
    private readonly HttpClient _client;

    public AuthTests(TestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task Register_ValidOrganiser_ReturnsOk()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Jane Organiser", email = "jane@test.com",
                  password = "Pass123!", role = "Organiser" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Bob", email = "bob@test.com",
                  password = "Pass123!", role = "Admin" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Dup", email = "dup@test.com",
                  password = "Pass123!", role = "Participant" });

        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Dup2", email = "dup@test.com",
                  password = "Pass123!", role = "Participant" });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Sam", email = "sam@test.com",
                  password = "Pass123!", role = "Participant" });

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { email = "sam@test.com", password = "Pass123!" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Sam2", email = "sam2@test.com",
                  password = "Pass123!", role = "Participant" });

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { email = "sam2@test.com", password = "WrongPass" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
