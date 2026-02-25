using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Machines.Commands;
using PristaneLaverieSmart.Application.Features.Machines.Commands.SetMachineStatus;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using Xunit;

public class SetMachineStatusHandlerTests
{
    [Fact]
    public async Task Handle_Should_SetStatus_WhenNoActiveBooking()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var id = Guid.NewGuid();
        var machine = new Machine { Id = id, Status = MachineStatus.Available, Name = "Washer", PricePerCycle = 6 };

        machines.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        bookings.Setup(r => r.HasActiveBookingNowAsync(id, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        machines.Setup(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SetMachineStatusHandler(machines.Object, bookings.Object);

        await handler.Handle(new SetMachineStatusCommand(id, MachineStatus.OutOfOrder), CancellationToken.None);

        machine.Status.Should().Be(MachineStatus.OutOfOrder);
        machines.Verify(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessRule_WhenActiveBookingAndSettingOutOfOrder()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var id = Guid.NewGuid();
        var machine = new Machine { Id = id, Status = MachineStatus.Available };

        machines.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        bookings.Setup(r => r.HasActiveBookingNowAsync(id, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new SetMachineStatusHandler(machines.Object, bookings.Object);

        var act = () => handler.Handle(new SetMachineStatusCommand(id, MachineStatus.OutOfOrder), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*OutOfOrder*");
    }
}