using System.Text;
using System.Text.Json;
using FluentAssertions;
using System.Net;

public class BookingApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingApiTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task PostBookings_Should_Return400_WhenMachineIdIsInvalidGuid()
    {
        var payload = new
        {
            machineId = "", // machine Id is invalid
            startTime = DateTimeOffset.UtcNow.AddHours(2),
            endTime = DateTimeOffset.UtcNow.AddHours(3),
            customerName = "Patrick"
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var res = await _client.PostAsync("/api/bookings", content);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);


        
    }

}