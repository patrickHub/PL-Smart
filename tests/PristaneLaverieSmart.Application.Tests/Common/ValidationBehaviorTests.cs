using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using FluentValidation;
using MediatR;
using PristaneLaverieSmart.Application.Common.Behaviors;
using PristaneLaverieSmart.Application.Features.Machines.Commands;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_Should_ThrowCustomValidationException_WhenValidationFalls()
    {
        var validators = new List<IValidator<CreateMachineCommand>>
        {
            new CreateMachineCommandValidator()
        };
        var behavior = new ValidationBehavior<CreateMachineCommand, Guid>(validators);
        var invalid = new CreateMachineCommand("", 6m);

        RequestHandlerDelegate<Guid> next = (_) => Task.FromResult(Guid.NewGuid());

        var act = () => behavior.Handle(invalid, next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<PristaneLaverieSmart.Application.Common.Exceptions.ValidationException>();
        ex.Which.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task Handle_should_CallNext_WhenNoValidators()
    {
        var behavior = new ValidationBehavior<CreateMachineCommand, Guid>(Enumerable.Empty<IValidator<CreateMachineCommand>>());
        var valid = new CreateMachineCommand("Washer", 6m);

        var called = false;
        RequestHandlerDelegate<Guid> next = (_) =>
        {
            called = true;
            return Task.FromResult(Guid.NewGuid());
        };

        var result = await behavior.Handle(valid, next, CancellationToken.None);
        called.Should().BeTrue();
        result.Should().NotBe(Guid.Empty);

    }

}