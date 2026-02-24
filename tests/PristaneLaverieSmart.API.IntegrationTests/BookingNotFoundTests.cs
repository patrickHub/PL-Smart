using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PristaneLaverieSmart.API.Contracts;

public class BookingsNotFoundTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsNotFoundTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostBookings_Should_Return404_WhenMachineDoesNotExist()
    {
        // Arrange
        var payload = new
        {
            machineId = Guid.NewGuid(), // machine does not exist
            startTime = DateTimeOffset.UtcNow.AddHours(2),
            endTime = DateTimeOffset.UtcNow.AddHours(3),
            customerName = "Patrick 404"
        };

        // Act
        var res = await _client.PostAsJsonAsync("/api/bookings", payload);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await res.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Status.Should().Be(404);
        error.Title.Should().Contain("not found");
        //error.Title.Should().Contain("not found", StringComparison.OrdinalIgnoreCase);
        error.TraceId.Should().NotBeNullOrWhiteSpace();
    }
}