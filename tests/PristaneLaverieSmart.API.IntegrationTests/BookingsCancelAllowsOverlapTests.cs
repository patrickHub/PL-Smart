using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

public class BookingsCancelAllowsOverlapTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsCancelAllowsOverlapTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse
    {
        public Guid Id { get; set; }
    }

    [Fact]
    public async Task CancelBooking_Should_AllowNewOverlappingBooking()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer Cancel", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Create booking1 [T+2, T+3]
        var start1 = DateTimeOffset.UtcNow.AddHours(2);
        var end1 = DateTimeOffset.UtcNow.AddHours(3);

        var createBooking1 = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = start1,
            endTime = end1,
            customerName = "Customer A"
        });

        createBooking1.StatusCode.Should().Be(HttpStatusCode.Created);
        var booking1 = await createBooking1.Content.ReadFromJsonAsync<CreateResponse>();
        var bookingId = booking1!.Id;

        // Cancel booking1
        var cancel = await _client.PostAsync($"/api/bookings/{bookingId}/cancel", content: null);
        cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Create overlapping booking2 [T+2.5, T+3.5] should now succeed
        var start2 = DateTimeOffset.UtcNow.AddHours(2.5);
        var end2 = DateTimeOffset.UtcNow.AddHours(3.5);

        var createBooking2 = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = start2,
            endTime = end2,
            customerName = "Customer B"
        });

        createBooking2.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}