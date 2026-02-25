using FluentAssertions;
using Moq;
using PristaneLaverieSmart.Application.Abstractions.Persistence;
using PristaneLaverieSmart.Application.Common.Exceptions;
using PristaneLaverieSmart.Application.Features.Bookings.Commands;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using SmartLaundry.Application.Features.Bookings.Commands;
using Xunit;

public class CompleteBookingHandlerTests
{
    [Fact]
    public async Task Handle_Should_CompleteReservedBooking()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        var booking = new Booking { Id = id, Status = BookingStatus.Received };

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        repo.Setup(r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CompleteBookingHandler(repo.Object);

        await handler.Handle(new CompleteBookingCommand(id), CancellationToken.None);

        booking.Status.Should().Be(BookingStatus.Completed);
        repo.Verify(r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFound_WhenBookingMissing()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var handler = new CompleteBookingHandler(repo.Object);

        var act = () => handler.Handle(new CompleteBookingCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Booking not found*");
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessRule_WhenAlreadyCompleted()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        var booking = new Booking { Id = id, Status = BookingStatus.Completed };

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CompleteBookingHandler(repo.Object);

        var act = () => handler.Handle(new CompleteBookingCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already completed*");
    }

    [Fact]
    public async Task Handle_Should_ThrowBusinessRule_WhenCancelled()
    {
        var repo = new Mock<IBookingRepository>();
        var id = Guid.NewGuid();

        var booking = new Booking { Id = id, Status = BookingStatus.Cancelled };

        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        var handler = new CompleteBookingHandler(repo.Object);

        var act = () => handler.Handle(new CompleteBookingCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*cannot be completed*");
    }
}