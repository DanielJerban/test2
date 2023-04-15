using BPMS.BuildingBlocks.Exceptions.Base;
using FluentValidation.Results;

namespace BPMS.BuildingBlocks.Exceptions.BussinessRule;

public class BussinessRuleFailedException : BaseException
{
    public BussinessRuleFailedException(string message) : base(message)
    {

    }
    public BussinessRuleFailedException(IEnumerable<ValidationFailure> errors) : base(string.Join(",\n", errors.Select(x => x.ErrorMessage)))
    {
    }

    protected override void SetStatusCode()
    {
        StatusCode = 422;//UNPROCESSABLE ENTITY
    }
}