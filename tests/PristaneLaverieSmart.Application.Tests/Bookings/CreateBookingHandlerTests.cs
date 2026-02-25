
using System.Reflection.Metadata;
using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CreateBooking;
using PristaneLaverieSmart.Domain.Entities;

public partial class CreateBookingHandlerTest
{
    
    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenMachineDoesNotExist()
    {

        var bookingRepos = new Mock<IBookingRepository>();
        var machineRepos = new Mock<IMachineRepository>();


        var handler = new CreateBookingHandler(bookingRepos.Object, machineRepos.Object);

        var cmd = new CreateBookingCommand(
            MachineId: Guid.NewGuid(),
            StartTime: DateTimeOffset.UtcNow.AddHours(2),
            EndTime: DateTimeOffset.UtcNow.AddHours(3),
            CustomerName: "Patrick"
        );

        var act = () => handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Machine not found*");
    
        bookingRepos.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);

        
    }
}