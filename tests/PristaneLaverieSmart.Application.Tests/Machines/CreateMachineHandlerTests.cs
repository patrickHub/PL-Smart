

using PristaneLaverieSmart.Domain.Entities;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Features.Machines.Commands;
using FluentAssertions;

public class CreateMachineHandlerTests
{
    [Fact]
    public async Task Handle_Should_CreateMachine_And_ReturnId()
    {
        var repos = new Mock<IMachineRepository>();

        Machine? saved = null;
        repos.Setup(r => r.AddAsync(It.IsAny<Machine>(), It.IsAny<CancellationToken>()))
            .Callback<Machine, CancellationToken>((m, _) => saved = m)
            .Returns(Task.CompletedTask);

        var handler = new CreateMachineHandler(repos.Object);

        var cmd = new CreateMachineCommand("Washer #999", 6.5m);

        var id = await handler.Handle(cmd, CancellationToken.None);

        id.Should().NotBe(Guid.Empty);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Washer #999");
        saved!.PricePerCycle.Should().Be(6.5m);

        repos.Verify(r => r.AddAsync(It.IsAny<Machine>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}