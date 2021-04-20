using Playground.Commands;
using Playground.Events;
using Playground.Contract;
using Playground.Persistence;
using Playground.Queries;

namespace Playground
{
    public static class ProgramAPI
    {
        static ProgramAPI()
        {
            using var db = new AppDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        } 
        
        public static EngageWithoutTransferAgreementResponse EngageWithoutTransferAgreement(EngageWithoutTransferAgreementRequest request)
        {
            using var db = new AppDbContext();
            var handler = new EngageWithoutTransferAgreementHandler(new TransferRepository(db));
            var response = handler.Handle(request);
            db.SaveChanges();
            return response;
        }  
        
        public static EngageWithTransferAgreementResponse EngageWithTransferAgreement(EngageWithTransferAgreementRequest request)
        {
            using var db = new AppDbContext();
            var handler = new EngageWithTransferAgreementHandler(new TransferInstructionRepository(db), new TransferRepository(db), db);
            var response = handler.Handle(request);
            db.SaveChanges();
            return response;
        }

        public static ReleasePlayerResponse ReleasePlayer(ReleasePlayerRequest request)
        {
            using var db = new AppDbContext();
            var response = new ReleasePlayerHandler(new TransferInstructionRepository(db), db, new TransferRepository(db))
                .Handle(request);
            db.SaveChanges();
            return response;
        }

        public static GetTransferByIdResponse GetTransferById(GetTransferByIdRequest request)
        {
            using var db = new AppDbContext();
            var handler = new GetTransferByIdQuery(db);
            return handler.Handle(request);
        }

        public static GetTransferInstructionByIdResponse GetTransferInstructionById(GetTransferInstructionByIdRequest request)
        {
            using var db = new AppDbContext();
            var handler = new GetTransferInstructionByIdQuery(db);
            return handler.Handle(request);
        }
    }
}
