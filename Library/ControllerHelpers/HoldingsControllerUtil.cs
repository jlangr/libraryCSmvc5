using Library.Models;
using System.Linq;

namespace Library.ControllerHelpers
{
    public class HoldingsControllerUtil
    {
        public static int NextAvailableCopyNumber(IRepository<Holding> holdingRepo, string classification)
        {
            var holdingsWithClassification = holdingRepo.FindBy(h => h.Classification == classification);
            return holdingsWithClassification.Count() + 1;
        }
    }
}