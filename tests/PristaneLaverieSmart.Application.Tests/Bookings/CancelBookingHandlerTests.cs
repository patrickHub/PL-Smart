using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.Application.Features.Bookings.Commands.CancelBooking;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using Xunit;

public class CancelBookingHandlerTests
{
    [Fact]
    public async Task Handle_Should_CancelReservedBooking()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        var booking = new Booking { Id = id, Status = BookingStatus.Received };

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        repo.Setup(r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CancelBookingHandler(repo.Object);

        await handler.Handle(new CancelBookingCommand(id), CancellationToken.None);

        booking.Status.Should().Be(BookingStatus.Cancelled);
        repo.Verify(r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenBookingMissing()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var handler = new CancelBookingHandler(repo.Object);

        var act = () => handler.Handle(new CancelBookingCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessRule_WhenAlreadyCancelled()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        var booking = new Booking { Id = id, Status = BookingStatus.Cancelled };

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CancelBookingHandler(repo.Object);

        var act = () => handler.Handle(new CancelBookingCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already cancelled*");
    }
}