using System;
using System.Collections.Generic;
using System.Linq;
using Library.Util;

namespace Library.Models
{
    public class HoldingService
    {
        private static readonly MultiMap<string, Holding> Holdings = new MultiMap<string, Holding>();

        public HoldingService()
        {
            ClassificationService = new MasterClassificationService();
        }

        public IClassificationService ClassificationService { private get; set; }

        public Holding Retrieve(string barcode)
        {
            var classification = Holding.ClassificationFromBarcode(barcode);
            return Holdings[classification].Where(x => x.Barcode == barcode).FirstOrDefault(); // ?DO: create Holdings collection, implement iterable
        }

        public Holding Add(string classification, int branchId)
        {
            var material = ClassificationService.Retrieve(classification);
            if (material == null)
                throw new LibraryException("Invalid classification");
            var copyNumber = Holdings.Count(classification) + 1;
            var holding = new Holding(classification, copyNumber, branchId) {CheckoutPolicy = material.CheckoutPolicy};
            Holdings.Add(classification, holding);
            return holding;
        }

        public void DeleteAllHoldings()
        {
            Holdings.Clear();
        }

        public bool IsCheckedOut(string barcode)
        {
            return Retrieve(barcode).IsCheckedOut;
        }

        public void CheckOut(DateTime timestamp, string barcode, int patronId, CheckoutPolicy checkoutPolicy)
        {
            Retrieve(barcode).CheckOut(timestamp, patronId, checkoutPolicy);
        }

        public void CheckIn(DateTime timestamp, string barcode, int branchId)
        {
            Retrieve(barcode).CheckIn(timestamp, branchId);
        }

        public IList<Holding> RetrieveAll()
        {
            return Holdings.Values();
        }
    }
}
