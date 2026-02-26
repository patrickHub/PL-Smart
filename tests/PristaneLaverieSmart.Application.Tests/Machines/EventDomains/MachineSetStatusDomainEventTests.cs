using FluentAssertions;
using PristaneLaverieSmart.Domain.Entities;
using PristaneLaverieSmart.Domain.Enums;
using PristaneLaverieSmart.Domain.Events;
using Xunit;

public class MachineSetStatusDomainEventTests
{
    [Fact]
    public void SetStatus_Should_RaiseExactlyOneDomainEvent_WhenStatusChanges()
    {
        var machine = new Machine { Status = MachineStatus.Available };

        machine.SetStatus(MachineStatus.Running);

        machine.DomainEvents.Should().HaveCount(1);
        var evt = machine.DomainEvents.Single().Should().BeOfType<MachineStatusChangedDomainEvent>().Subject;

        evt.OldStatus.Should().Be(MachineStatus.Available);
        evt.NewStatus.Should().Be(MachineStatus.Running);
        evt.MachineId.Should().Be(machine.Id);
    }

    [Fact]
    public void SetStatus_Should_RaiseNoEvent_WhenStatusDoesNotChange()
    {
        var machine = new Machine { Status = MachineStatus.Available };

        machine.SetStatus(MachineStatus.Available);

        machine.DomainEvents.Should().BeEmpty();
    }
}