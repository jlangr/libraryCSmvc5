using System.Linq;

namespace Library.Models.Repositories
{
    // TODO Can we use a delegate here to handle both repo types?
    public class HoldingRepositoryExtensions
    {
        // TODO test!
        public static Holding XFindByBarcode(IRepository<Holding> repo, string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var copyNumber = Holding.CopyNumberFromBarcode(barcode);
            var results = repo.FindBy(
                (holding => {
                    return holding.Classification == classification && holding.CopyNumber == copyNumber;
                }));
            return results.FirstOrDefault();
        }
    }
}