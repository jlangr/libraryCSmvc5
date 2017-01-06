using System.Linq;

namespace Library.Models.Repositories
{
    public class HoldingRepository : EntityRepository<Holding>
    {
        public HoldingRepository() : base(db => db.Holdings) { }

        // TODO test
        public Holding FindByBarcode(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var copyNumber = Holding.CopyNumberFromBarcode(barcode);
            return dbSetFunc(db).FirstOrDefault(holding => holding.Classification == classification && holding.CopyNumber == copyNumber);
        }
    }
}