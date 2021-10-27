using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersService.Persistence;

namespace TransfersService.Commands
{
    internal class EngageWithoutTransferAgreement : IRequestHandler<Contract.EngageWithoutTransferAgreement.Request, Contract.EngageWithoutTransferAgreement.Response>
    {
        private readonly TransferRepository _transferRepository;

        public EngageWithoutTransferAgreement(TransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task<Contract.EngageWithoutTransferAgreement.Response> Handle(Contract.EngageWithoutTransferAgreement.Request request, CancellationToken ct)
        {
            var transfer = Domain.Transfer.CreateNewEngageWithoutTransferAgreement(request);

            await _transferRepository.Add(transfer, ct);

            return new Contract.EngageWithoutTransferAgreement.Response
            {
                TransferId = transfer.Id
            };
        }
    }

}