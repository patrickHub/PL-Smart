using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;

public class BookingsEndToEndTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsEndToEndTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse
    {
        public Guid Id { get; set; }
    }

    private sealed class BookingDto
    {
        public Guid Id { get; set; }
        public Guid MachineId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string CustomerName { get; set; } = "";
        public String Status { get; set; } = "";
    }

    [Fact]
    public async Task EndToEnd_Should_CreateMachine_CreateBooking_AndReturnItInList()
    {
        // 1) Create a machine
        var machinePayload = new { name = "Washer E2E", pricePerCycle = 6.0m };

        var createMachineRes = await _client.PostAsJsonAsync("/api/machines", machinePayload);
        createMachineRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var machineCreated = await createMachineRes.Content.ReadFromJsonAsync<CreateResponse>();
        machineCreated.Should().NotBeNull();
        machineCreated!.Id.Should().NotBe(Guid.Empty);

        // 2) Create a booking for that machine
        var start = DateTimeOffset.UtcNow.AddHours(2);
        var end = DateTimeOffset.UtcNow.AddHours(3);

        var bookingPayload = new
        {
            machineId = machineCreated.Id,
            startTime = start,
            endTime = end,
            customerName = "Patrick E2E"
        };

        var createBookingRes = await _client.PostAsJsonAsync("/api/bookings", bookingPayload);
        createBookingRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var bookingCreated = await createBookingRes.Content.ReadFromJsonAsync<CreateResponse>();
        bookingCreated.Should().NotBeNull();
        bookingCreated!.Id.Should().NotBe(Guid.Empty);

        // 3) Get bookings and verify it is present
        var listRes = await _client.GetAsync("/api/bookings");
        listRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var bookings = await listRes.Content.ReadFromJsonAsync<List<BookingDto>>();
        bookings.Should().NotBeNull();
        bookings!.Should().Contain(b =>
            b.Id == bookingCreated.Id &&
            b.MachineId == machineCreated.Id &&
            b.CustomerName == bookingPayload.customerName
        );
    }
}