using FluentAssertions;
using Microsoft.OpenApi.Expressions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class MachinesApiTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MachinesApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostMachines_Should_Return400_WhenNameIsEmpty()
    {
        // Arrange
        var payload = new {name="", pricePerCycle=6};

        // Act
        var res = await _client.PostAsJsonAsync("/api/machines", payload);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await res.Content.ReadAsStringAsync();
        json.Should().Contain("Validation failed");
        json.Should().Contain("Name");

    }

    [Fact]
    public async Task PostMachine_Should_Create_AndThenGetMachinesReturnsIt()
    {
        // Arrange
        var payload = new {name="Washer INT", pricePerCycle=6};

        // Act
        var create = await _client.PostAsJsonAsync("/api/machines", payload);

        // Assert
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var list = await _client.GetAsync("/api/machines");
        list.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await list.Content.ReadAsStringAsync();
        body.Should().Contain("Washer INT");

    }
}