using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

public class MachinesGetByIdTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MachinesGetByIdTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private sealed class CreateResponse { public Guid Id { get; set; } }

    private sealed class MachineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal PricePerCycle { get; set; }
        public string Status { get; set; } = "";
    }

    [Fact]
    public async Task GetMachineById_Should_ReturnMachine()
    {
        var create = await _client.PostAsJsonAsync("/api/machines", new { name = "Washer ById", pricePerCycle = 6m });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await create.Content.ReadFromJsonAsync<CreateResponse>();
        var id = created!.Id;

        var res = await _client.GetAsync($"/api/machines/{id}");
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var machine = await res.Content.ReadFromJsonAsync<MachineDto>();
        machine.Should().NotBeNull();
        machine!.Id.Should().Be(id);
        machine.Name.Should().Be("Washer ById");
    }
}