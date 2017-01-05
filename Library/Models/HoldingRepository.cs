using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class HoldingRepository : EntityRepository<Holding>
    {
        public HoldingRepository() : base(db => db.Holdings) { }

        // TODO test
        public Holding FindByBarcode(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            var copyNumber = Holding.CopyNumberFromBarcode(barcode);
            // TODO can this logic be pushed into Holding
            return dbSetFunc(db).First(holding => holding.Classification == classification && holding.CopyNumber == copyNumber);
        }
    }
}