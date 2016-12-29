using System;

namespace Library.Models
{
    public class Holding
    {
        public const int NoPatron = -1;
        public const int CheckedOutBranchId = -1;

        public Holding(string classification, int copyNumber) 
            : this(classification, copyNumber, CheckedOutBranchId)
        {
        }

        public Holding(string classification, int copyNumber, int branchId)
        {
            CheckOutTimestamp = DateTime.MinValue;
            LastCheckedIn = DateTime.MinValue;
            DueDate = DateTime.MinValue;
            Classification = classification;
            CopyNumber = copyNumber;
            BranchId = branchId;
        }

        public string Classification { get; private set; }

        public string Barcode
        {
            get
            {
                return GenerateBarcode(Classification, CopyNumber);
            }
        }

        public DateTime DueDate { get; private set; }

        public bool IsCheckedOut
        {
            get
            {
                return CheckOutTimestamp != DateTime.MinValue;
            }
        }

        public void CheckIn(DateTime timestamp, int toBranchId)
        {
            LastCheckedIn = timestamp;
            CheckOutTimestamp = DateTime.MinValue;
            HeldByPatronId = NoPatron;
            BranchId = toBranchId;
        }

        public DateTime LastCheckedIn { get; private set; }

        public void CheckOut(DateTime timestamp, int patronId, CheckoutPolicy checkoutPolicy)
        {
            CheckOutTimestamp = timestamp;
            HeldByPatronId = patronId;
            CheckoutPolicy = checkoutPolicy;
            CalculateDueDate();
        }

        private void CalculateDueDate()
        {
            DueDate = CheckOutTimestamp.AddDays(CheckoutPolicy.MaximumCheckoutDays());
        }

        public CheckoutPolicy CheckoutPolicy { get; set; }

        public DateTime CheckOutTimestamp { get; private set; }

        public int CopyNumber { get; private set; }

        public int BranchId { get; set; }

        public int HeldByPatronId { get; private set; }

        public static string GenerateBarcode(string classification, int copyNumber)
        {
            return String.Format("{0}:{1}", classification, copyNumber);
        }

        public static string ClassificationFromBarcode(string barcode)
        {
            var colonIndex = barcode.IndexOf(':');
            return barcode.Substring(0, colonIndex);
        }
    }
}
