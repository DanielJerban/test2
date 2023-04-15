using FluentValidation;

namespace BPMS.Application.CQRS.Sample.Create;

public class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
{
    public CreateSampleCommandValidator()
    {
        RuleFor(c => c.Name).NotNull().WithMessage("The name field is required");
    }
}