using BPMS.BuildingBlocks.Exceptions.BussinessRule;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;

namespace BPMS.BuildingBlocks.Infrastructure.Configuration.Processing;

public class CommandHandlerFluentValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public CommandHandlerFluentValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();
            if (failures.Any()) throw new BussinessRuleFailedException(failures);
        }
    }
}