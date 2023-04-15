using BPMS.Application.CQRS.Sample.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly IMediator _mediator;

    public SampleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public Task<CreateSampleCommandResult> Create(string name, CancellationToken cancellationToken)
        => _mediator.Send(new CreateSampleCommand(name), cancellationToken);
}