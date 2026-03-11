using PristaneLaverieSmart.Domain.Common;

namespace PristaneLaverieSmart.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}