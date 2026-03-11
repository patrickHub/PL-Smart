using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PristaneLaverieSmart.API.Contracts;
using Xunit;
using Xunit.Abstractions;

public class BookingsOverlapIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public BookingsOverlapIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    private sealed class CreateResponse
    {
        public Guid Id { get; set; }
    }

    [Fact]
    public async Task PostBookings_Should_Return409_WhenOverlappingBookingExists()
    {
        // 1) Create a machine
        var machinePayload = new { name = "Washer Overlap", pricePerCycle = 6.0m };

        var createMachineRes = await _client.PostAsJsonAsync("/api/machines", machinePayload);
        createMachineRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var machineCreated = await createMachineRes.Content.ReadFromJsonAsync<CreateResponse>();
        machineCreated.Should().NotBeNull();
        var machineId = machineCreated!.Id;
        machineId.Should().NotBe(Guid.Empty);

        // 2) Create first booking: [T+2h, T+3h]
        var start1 = DateTimeOffset.UtcNow.AddHours(2);
        var end1 = DateTimeOffset.UtcNow.AddHours(3);

        var booking1 = new
        {
            machineId,
            startTime = start1,
            endTime = end1,
            customerName = "Customer 1"
        };

        _output.WriteLine($"Created Booking JSON: {booking1}");

        var res1 = await _client.PostAsJsonAsync("/api/bookings", booking1);
        var crest = await res1.Content.ReadFromJsonAsync<CreateResponse>();
        string jsonDebug = JsonSerializer.Serialize(crest, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        _output.WriteLine($"Created Booking JSON: {jsonDebug}");
        res1.StatusCode.Should().Be(HttpStatusCode.Created);

        // 3) Create overlapping booking: [T+2h30, T+3h30] overlaps
        var start2 = DateTimeOffset.UtcNow.AddHours(2.5);
        var end2 = DateTimeOffset.UtcNow.AddHours(3.5);

        var booking2 = new
        {
            machineId,
            startTime = start2,
            endTime = end2,
            customerName = "Customer 2"
        };

        var res2 = await _client.PostAsJsonAsync("/api/bookings", booking2);

        // Assert 409 + error contract
        res2.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await res2.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Status.Should().Be(409);
        error.Title.Should().ContainEquivalentOf("Business rule");
        error.Detail.Should().ContainEquivalentOf("overlap");
        error.TraceId.Should().NotBeNullOrWhiteSpace();
    }
}