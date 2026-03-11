using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PristaneLaverieSmart.API.Contracts;
using Xunit;

public class BookingsCompleteIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsCompleteIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    [Fact]
    public async Task CompleteBooking_Should_Return204_AndThenCancelShouldReturn409()
    {
        // Create machine
        var createMachine = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer Complete", pricePerCycle = 6m });
        createMachine.StatusCode.Should().Be(HttpStatusCode.Created);
        var machine = await createMachine.Content.ReadFromJsonAsync<CreateResponse>();
        var machineId = machine!.Id;

        // Create booking
        var start = DateTimeOffset.UtcNow.AddHours(2);
        var end = DateTimeOffset.UtcNow.AddHours(3);

        var createBooking = await _client.PostAsJsonAsync("/api/bookings", new
        {
            machineId,
            startTime = start,
            endTime = end,
            customerName = "Customer Complete"
        });

        createBooking.StatusCode.Should().Be(HttpStatusCode.Created);
        var booking = await createBooking.Content.ReadFromJsonAsync<CreateResponse>();
        var bookingId = booking!.Id;

        // Complete booking
        var complete = await _client.PostAsync($"/api/bookings/{bookingId}/complete", content: null);
        complete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try cancel afterwards -> 409 (Completed bookings cannot be cancelled)
        var cancel = await _client.PostAsync($"/api/bookings/{bookingId}/cancel", content: null);
        cancel.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await cancel.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Status.Should().Be(409);
        error.Title.Should().Contain("Business Rule");
    }
}