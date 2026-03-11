using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;
using PristaneLaverieSmart.Application.Features.Bookings.EventHandlers;
using Xunit;

public class BookingCreatedHandlerTests
{
    [Fact]
    public async Task Handle_Should_SetMachineToRunning_WhenBookingActiveAndMachineNotOutOfOrder()
    {
        // Arrange
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var machineId = Guid.NewGuid();
        var machine = new Machine { Id = machineId, Status = MachineStatus.Available };

        machines.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        bookings.Setup(r => r.HasActiveBookingNowAsync(machineId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        machines.Setup(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingCreatedHandler(machines.Object, bookings.Object);

        var evt = new BookingCreatedDomainEvent(
            BookingId: Guid.NewGuid(),
            MachineId: machineId,
            StartTime: DateTimeOffset.UtcNow,
            EndTime: DateTimeOffset.UtcNow.AddHours(1)
        );

        // Act
        await handler.Handle(evt, CancellationToken.None);

        // Assert
        machine.Status.Should().Be(MachineStatus.Running);
        machines.Verify(r => r.UpdateAsync(machine, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotChangeMachine_WhenOutOfOrder()
    {
        var machines = new Mock<IMachineRepository>();
        var bookings = new Mock<IBookingRepository>();

        var machineId = Guid.NewGuid();
        var machine = new Machine { Id = machineId, Status = MachineStatus.OutOfOrder };

        machines.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(machine);

        var handler = new BookingCreatedHandler(machines.Object, bookings.Object);

        var evt = new BookingCreatedDomainEvent(Guid.NewGuid(), machineId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

        await handler.Handle(evt, CancellationToken.None);

        machines.Verify(r => r.UpdateAsync(It.IsAny<Machine>(), It.IsAny<CancellationToken>()), Times.Never);
        machine.Status.Should().Be(MachineStatus.OutOfOrder);
    }
}