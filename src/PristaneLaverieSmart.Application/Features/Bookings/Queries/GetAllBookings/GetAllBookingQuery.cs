using MediatR;
using PristaneLaverieSmart.Application.Features.Dtos;

namespace PristaneLaverieSmart.Application.Features.Bookings.Query;

public sealed record GetAllBookingQuerry: IRequest<IReadOnlyList<BookingDto>>;