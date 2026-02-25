using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using Xunit;

public class CreateBookingBusinessRulesTests
{
    [Fact]
    public async Task Handle_Should_ThrowBusinessRuleException_WhenOverlapExists()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var machineRepo = new Mock<IMachineRepository>();

        var machineId = Guid.NewGuid();

        machineRepo.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Machine { Id = machineId, Name = "Washer", PricePerCycle = 6, Status = MachineStatus.Available });

        bookingRepo.Setup(r => r.HasOverlapAsync(machineId, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateBookingHandler(bookingRepo.Object, machineRepo.Object);

        var cmd = new CreateBookingCommand(
            machineId,
            DateTimeOffset.UtcNow.AddHours(2),
            DateTimeOffset.UtcNow.AddHours(3),
            "Patrick"
        );

        var act = () => handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*overlaps*");
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessRuleException_WhenMachineIsOutOfOrder()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var machineRepo = new Mock<IMachineRepository>();

        var machineId = Guid.NewGuid();

        machineRepo.Setup(r => r.GetByIdAsync(machineId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Machine { Id = machineId, Name = "Washer", PricePerCycle = 6, Status = MachineStatus.OutOfOrder });

        var handler = new CreateBookingHandler(bookingRepo.Object, machineRepo.Object);

        var cmd = new CreateBookingCommand(
            machineId,
            DateTimeOffset.UtcNow.AddHours(2),
            DateTimeOffset.UtcNow.AddHours(3),
            "Patrick"
        );

        var act = () => handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*OutOfOrder*");
    }
}