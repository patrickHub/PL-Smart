using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Features.Bookings.EventHandlers;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;
using Xunit;

public class BookingEndedHandlerTests
{
    [Fact]
    public async Task Cancelled_Should_SetAvailable_WhenNoActiveBooking()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var machineId = Guid.NewGuid();
        var machine = new Machine { Id = machineId, Status = MachineStatus.Running };

        machines.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        bookings.Setup(r => r.HasActiveBookingNowAsync(machineId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        machines.Setup(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingEndedHandler(machines.Object, bookings.Object);

        await handler.Handle(new BookingCancelledDomainEvent(Guid.NewGuid(), machineId), CancellationToken.None);

        machine.Status.Should().Be(MachineStatus.Available);
        machines.Verify(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Completed_Should_KeepRunning_WhenAnotherActiveBookingExists()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var machineId = Guid.NewGuid();
        var machine = new Machine { Id = machineId, Status = MachineStatus.Running };

        machines.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        bookings.Setup(r => r.HasActiveBookingNowAsync(machineId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        machines.Setup(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingEndedHandler(machines.Object, bookings.Object);

        await handler.Handle(new BookingCompletedDomainEvent(Guid.NewGuid(), machineId), CancellationToken.None);

        machine.Status.Should().Be(MachineStatus.Running);
        machines.Verify(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Ended_Should_DoNothing_WhenMachineOutOfOrder()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var machineId = Guid.NewGuid();
        var machine = new Machine { Id = machineId, Status = MachineStatus.OutOfOrder };

        machines.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        var handler = new BookingEndedHandler(machines.Object, bookings.Object);

        await handler.Handle(new BookingCompletedDomainEvent(Guid.NewGuid(), machineId), CancellationToken.None);

        machines.Verify(r => r.UpdateAsync(It.IsAny<Machine>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}