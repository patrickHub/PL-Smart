using MediatR;
namespace PristaneLaverieSmart.Domain.Common;

public interface IDomainEvent: INotification
{
    DateTimeOffset OccuredOn {get;}
}