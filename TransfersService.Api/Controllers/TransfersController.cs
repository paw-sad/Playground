using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TransfersService.Contract;

namespace TransfersService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ILogger<TransfersController> _logger;
        private readonly IMediator _mediator;

        public TransfersController(ILogger<TransfersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<EngageWithoutTransferAgreement.Response> EngageWithoutTransferAgreement(EngageWithoutTransferAgreement.Request request)
        {
            return await _mediator.Send(request);
        }
    }
}
