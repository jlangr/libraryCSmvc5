using Library.Models;
using System.Linq;

namespace Library.ControllerHelpers
{
    public class HoldingsControllerUtil
    {
        public static Holding FindByClassificationAndCopy(IRepository<Holding> repo, string classification, int copyNumber)
        {
            var results = repo.FindBy(
                (holding => { return holding.Classification == classification && holding.CopyNumber == copyNumber; }));
            return results.FirstOrDefault();
        }

        public static Holding FindByBarcode(IRepository<Holding> repo, string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var copyNumber = Holding.CopyNumberFromBarcode(barcode);
            var results = repo.FindBy(
                (holding => { return holding.Classification == classification && holding.CopyNumber == copyNumber; }));
            return results.FirstOrDefault();
        }

        public static int NextAvailableCopyNumber(IRepository<Holding> holdingRepo, string classification)
        {
            var holdingsWithClassification = holdingRepo.FindBy(h => h.Classification == classification);
            return holdingsWithClassification.Count() + 1;
        }
    }
}